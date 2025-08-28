
using LibVLCSharp.Shared;
using odm.core;
using onvif.services;
using onvif.utils;
using Pal.Helper.Camera;
using SharpOnvifClient;
using SharpOnvifCommon;
using SharpOnvifCommon.Security;
using System;
using System.Diagnostics;
using System.Net;
using System.Reactive.Disposables;
using System.Security.Principal;
using utils;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace OnvifCamera
{
    public partial class MainForm : Form
    {
        LibVLC vlc;
        MediaPlayer player;
        string DebugText {  get; set; }
        public MainForm()
        {
            InitializeComponent();
            IpTextBox.Text = "192.168.0.10:554";
            UserTextBox.Text = "admin";
            PasswordTextBox.Text = "admin";
            DebugText = string.Empty;

            TextBox.CheckForIllegalCrossThreadCalls = false;
        }

        private async void DiscoverButton_Click(object sender, EventArgs e)
        {
            Onvif onvif = new Onvif();
            try
            {
                var onvifDeviceUris = await onvif.DiscoverAsync();
                comboBox1.Items.Clear();
                if(onvifDeviceUris!= null && onvifDeviceUris.Any())
                {
                    comboBox1.Items.AddRange(onvifDeviceUris.ToArray());
                    
                }
                else
                {
                    MessageBox.Show("No devices found");
                    return;
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private void DisplayButton_Click(object sender, EventArgs e)
        {

            Core.Initialize();
            vlc = new LibVLC();
            player = new MediaPlayer(vlc);
            videoView.MediaPlayer = player;

            var url = UrlTextBox.Text;
            var user = UserTextBox.Text;
            var password = PasswordTextBox.Text;
            var ip = IpTextBox.Text;
            if (string.IsNullOrEmpty(url))
            {

                url = $"rtsp://{user}:{password}@{ip}/1/1";
            }
            player.Play(new Media(vlc, new Uri(url)));
        }

        private async void ConnectEventsButton_Click(object sender, EventArgs e)
        {
            var tempIp = IpTextBox.Text;
            var ip = IpTextBox.Text;
            if (tempIp.Contains(":")) 
            {
                string[] ips = tempIp.Split(':');
                if(ips.Length > 0)
                {
                    ip = ips[0];
                }
            }

            var url = UrlTextBox.Text;
            var user = UserTextBox.Text;
            var password = PasswordTextBox.Text;
            if (string.IsNullOrEmpty(url))
            {

                url = $"http://{ip}/";
            }           
            var account = new NetworkCredential(user, password);
            SampleRunner runner = new SampleRunner(new Uri(url), account, StatusTextBox);  
        }

        static async Task PullPointEventSubscription(SimpleOnvifClient client)
        {
            var subscription = await client.PullPointSubscribeAsync();
            while (true)
            {
                var messages = await client.PullPointPullMessagesAsync(subscription);

                foreach (var ev in messages.NotificationMessage)
                {
                    if (OnvifEvents.IsMotionDetected(ev) != null)
                        Console.WriteLine($"Motion detected: {OnvifEvents.IsMotionDetected(ev)}");
                    else if (OnvifEvents.IsTamperDetected(ev) != null)
                        Console.WriteLine($"Tamper detected: {OnvifEvents.IsTamperDetected(ev)}");
                }
            }
        }

        static async Task BasicEventSubscription(SimpleOnvifClient client)
        {
            // we must run as an Administrator for the Basic subscription to work
            var eventListener = new SimpleOnvifEventListener();
            eventListener.Start((int cameraID, string ev) =>
            {
                if (OnvifEvents.IsMotionDetected(ev) != null)
                    Console.WriteLine($"Motion detected: {OnvifEvents.IsMotionDetected(ev)}");
                else if (OnvifEvents.IsTamperDetected(ev) != null)
                    Console.WriteLine($"Tamper detected: {OnvifEvents.IsTamperDetected(ev)}");
            });

            var subscriptionResponse = await client.BasicSubscribeAsync(eventListener.GetOnvifEventListenerUri());

            while (true)
            {
                await Task.Delay(1000 * 60 * 4);
                var result = await client.BasicSubscriptionRenewAsync(subscriptionResponse);
            }
        }

        public class SampleRunner : IDisposable
        {
            public SampleRunner(Uri uri, NetworkCredential account, TextBox debugTextBox)
            {                

                Init(uri, account, debugTextBox);
            }
            CompositeDisposable disposables = new CompositeDisposable();

            private void Init(Uri uri, NetworkCredential account, TextBox debugTextBox)
            {
                NvtSessionFactory factory = new NvtSessionFactory(account);

                disposables.Add(factory.CreateSession(new[] { uri })
                    .Subscribe(
                    session => {
                        disposables.Add(new EventManager(session,debugTextBox));
                    }, err => {
                        
                        Console.WriteLine(err.Message);
                    }
                ));
            }

            public void Dispose()
            {
                disposables.Dispose();
            }
        }

        public class EventManager : IDisposable
        {
            public TextBox DebugTextBox { get; set; }
            public EventManager(INvtSession session, TextBox debugTextBox)
            {
                // TODO: Complete member initialization
                this.session = session;

                Run(debugTextBox);
            }

            private INvtSession session;
            CompositeDisposable disposables = new CompositeDisposable();

            public void Dispose()
            {
                disposables.Dispose();
            }

            private void Run(TextBox debugTextBox)
            {
                OdmSession odmSess = new OdmSession(session);
                DebugTextBox = debugTextBox;

                disposables.Add(odmSess.GetPullPointEvents()
                    .Subscribe(
                    evnt => {
                        var message = "----------------------------------------" + System.Environment.NewLine;
                        message +=evnt.ToString() + System.Environment.NewLine;
                        message += EventParse.ParseTopic(evnt.topic) + System.Environment.NewLine;
                        
                        var messages = EventParse.ParseMessage(evnt.message);
                        
                        messages.ForEach(msg => 
                        { debugTextBox.Text += msg + System.Environment.NewLine;
                            if (msg.ToString().Contains("AlarmIn"))
                            {
                                debugTextBox.Text += $"{System.DateTime.Now.ToString()} : ***********************ALARM IN*********************" + System.Environment.NewLine;
                            }

                        });
                        debugTextBox.Text += "----------------------------------------" + System.Environment.NewLine;                      
                    }, err => {
                        debugTextBox.Text += err.Message + System.Environment.NewLine;
                    }
                ));
            }
        }
        public static class EventParse
        {
            public static string ParseTopic(TopicExpressionType topic)
            {
                string topicString = "";

                topic.Any.ForEach(node => {
                    topicString += "value: " + node.Value;
                });

                return topicString;
            }
            public static string[] ParseMessage(onvif.services.Message message)
            {
                List<string> messageStrings = new List<string>();

                messageStrings.Add("messge id: " + message.key);

                if (message.source != null)
                    message.source.simpleItem.ForEach(sitem => {
                        string txt = sitem.name + "	" + sitem.value;
                        messageStrings.Add(txt);
                    });

                if (message.data != null)
                    message.data.simpleItem.ForEach(sitem => {
                        string txt = sitem.name + "	" + sitem.value;
                        messageStrings.Add(txt);
                    });

                return messageStrings.ToArray();
            }
        }
        public bool DebugUpdate(string msg) 
        { 
            DebugText += msg + Environment.NewLine;
            StatusTextBox.Text = DebugText;
            StatusTextBox.SelectionStart = StatusTextBox.Text.Length;
            StatusTextBox.ScrollToCaret();
            return true;
        }        
    }
}

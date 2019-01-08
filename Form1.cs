using MQTTnet;
using MQTTnet.Core;
using MQTTnet.Core.Client;
using MQTTnet.Core.Packets;
using MQTTnet.Core.Protocol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mqtt_client
{
    public partial class Form1 : Form
    {
        private MqttClient mqttClient = null;
        public Form1()
        {
            InitializeComponent();
            Task.Run(async () => { await ConnectMqttServerAsync(); });

        }
        private async Task ConnectMqttServerAsync()
        {
            if (mqttClient == null)
            {
                mqttClient = new MqttClientFactory().CreateMqttClient() as MqttClient;
                mqttClient.ApplicationMessageReceived += MqttClient_ApplicationMessageReceived;
                mqttClient.Connected += MqttClient_Connected;
                mqttClient.Disconnected += MqttClient_Disconnected;
            }

            try
            {
                var options = new MqttClientTcpOptions
                {
                    Server = "127.0.0.1",
                    ClientId = Guid.NewGuid().ToString(),
                    UserName = "123",
                    Password = "123",
                    CleanSession = true
                };

                await mqttClient.ConnectAsync(options);
            }
            catch (Exception ex)
            {
                Invoke((new Action(() =>
                {
                    textBox2.AppendText($"连接到MQTT服务器失败！" + Environment.NewLine + ex.Message + Environment.NewLine);
                })));
            }
        }
        private void MqttClient_Connected(object sender, EventArgs e)
        {
            Invoke((new Action(() =>
            {
                textBox2.AppendText("已连接到MQTT服务器！" + Environment.NewLine);
            })));
        }

        private void MqttClient_Disconnected(object sender, EventArgs e)
        {
            Invoke((new Action(() =>
            {
                textBox2.AppendText("已断开MQTT连接！" + Environment.NewLine);
            })));
        }

        private void MqttClient_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            Invoke((new Action(() =>
            {
                textBox2.AppendText($">> {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}{Environment.NewLine}");
            })));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string topic = textBox1.Text.Trim();

            if (string.IsNullOrEmpty(topic))
            {
                MessageBox.Show("订阅主题不能为空！");
                return;
            }

            if (!mqttClient.IsConnected)
            {
                MessageBox.Show("MQTT客户端尚未连接！");
                return;
            }

            mqttClient.SubscribeAsync(new List<TopicFilter> {
                new TopicFilter(topic, MqttQualityOfServiceLevel.AtMostOnce)
            });

            textBox2.AppendText($"已订阅[{topic}]主题" + Environment.NewLine);
            textBox1.Enabled = false;
            button1.Enabled = false;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string topic = textBox4.Text.Trim();

            if (string.IsNullOrEmpty(topic))
            {
                MessageBox.Show("发布主题不能为空！");
                return;
            }

            string inputString = textBox3.Text.Trim();
            var appMsg = new MqttApplicationMessage(topic, Encoding.UTF8.GetBytes(inputString), MqttQualityOfServiceLevel.AtMostOnce, false);
            mqttClient.PublishAsync(appMsg);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Sunisoft.IrisSkin.SkinEngine skin = new Sunisoft.IrisSkin.SkinEngine
            {
                SkinFile = @"./RealOne.ssk"
            };
        }
    }
}

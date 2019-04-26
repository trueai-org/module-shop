using EasyNetQ;
using EasyNetQ.Topology;
using Shop.Module.Core.Abstractions.Services;
using Shop.Module.MQ.Abstractions.Services;
using Shop.Module.RabbitMQ.Models;
using System;
using System.Threading.Tasks;

namespace Shop.Module.RabbitMQ.Services
{
    public class MQService : IMQService
    {
        private readonly IBus _bus;
        public MQService(IAppSettingService appSettingService)
        {
            var options = appSettingService.Get<RabbitMQOptions>().Result;
            if (string.IsNullOrWhiteSpace(options?.ConnectionString))
                throw new ArgumentNullException(nameof(RabbitMQOptions));

            _bus = RabbitHutch.CreateBus(options.ConnectionString);
        }

        //-------------extens

        private IQueue CreateQueue(IAdvancedBus adbus, string queueName = "")
        {
            if (adbus == null) return null;
            if (string.IsNullOrEmpty(queueName)) return adbus.QueueDeclare();
            return adbus.QueueDeclare(queueName);
        }

        public void TopicSubscribe<T>(string subscriptionId, Action<T> callback, params string[] topics) where T : class
        {
            using (var bus = _bus)
            {
                bus.Subscribe(subscriptionId, callback, (config) =>
                {
                    foreach (var item in topics)
                    {
                        config.WithTopic(item);
                    }
                });
            }
        }

        public bool TopicPublish<T>(string topic, T message, out string msg) where T : class
        {
            msg = string.Empty;
            try
            {
                using (var bus = _bus)
                {
                    bus.Publish(message, topic);
                    return true;
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                return false;
            }
        }

        public bool TopicSubscribe<T>(T t, string topic, out string msg, string exChangeName = "topic_mq") where T : class
        {
            msg = string.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(topic))
                    throw new Exception("topic is empty.");
                using (var bus = _bus)
                {
                    var adbus = bus.Advanced;
                    //var queue = adbus.QueueDeclare("user.notice.zhangsan");
                    var exchange = adbus.ExchangeDeclare(exChangeName, ExchangeType.Topic);
                    adbus.Publish(exchange, topic, false, new Message<T>(t));
                    return true;
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                return false;
            }
        }

        public bool FanoutPush<T>(T t, out string msg, string exChangeName = "fanout_mq", string routingKey = "") where T : class
        {
            msg = string.Empty;
            try
            {
                using (var bus = _bus)
                {
                    var adbus = bus.Advanced;
                    var exchange = adbus.ExchangeDeclare(exChangeName, ExchangeType.Fanout);
                    adbus.Publish(exchange, routingKey, false, new Message<T>(t));
                    return true;
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                return false;
            }
        }

        public void FanoutConsume<T>(Action<T> handler, string exChangeName = "fanout_mq", string queueName = "fanout_queue_default", string routingKey = "") where T : class
        {
            var bus = _bus;
            var adbus = bus.Advanced;
            var exchange = adbus.ExchangeDeclare(exChangeName, ExchangeType.Fanout);
            var queue = CreateQueue(adbus, queueName);
            adbus.Bind(exchange, queue, routingKey);
            adbus.Consume(queue, registration =>
            {
                registration.Add<T>((message, info) =>
                {
                    handler(message.Body);
                });
            });
        }

        #region "direct"
        /// <summary>
        /// 消息发送（direct）
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="queue">发送到的队列</param>
        /// <param name="message">发送内容</param>
        public async Task DirectSend<T>(string queue, T message) where T : class
        {
            await _bus.SendAsync(queue, message);
        }

        /// <summary>
        /// 消息接收（direct）
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="queue">接收的队列</param>
        /// <param name="callback">回调操作</param>
        /// <param name="msg">错误信息</param>
        /// <returns></returns>
        public bool DirectReceive<T>(string queue, Action<T> callback, out string msg) where T : class
        {
            msg = string.Empty;
            try
            {
                _bus.Receive(queue, callback);
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                return false;
            }
            return true;
        }

        /// <summary>
        /// 消息发送
        /// <![CDATA[（direct EasyNetQ高级API）]]>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="msg"></param>
        /// <param name="exChangeName"></param>
        /// <param name="routingKey"></param>
        /// <returns></returns>
        public bool DirectPush<T>(T t, out string msg, string exChangeName = "direct_mq", string routingKey = "direct_rout_default") where T : class
        {
            msg = string.Empty;
            try
            {
                using (var bus = _bus)
                {
                    var adbus = bus.Advanced;
                    var exchange = adbus.ExchangeDeclare(exChangeName, ExchangeType.Direct);
                    adbus.Publish(exchange, routingKey, false, new Message<T>(t));
                    return true;
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                return false;
            }
        }
        /// <summary>
        /// 消息接收
        ///  <![CDATA[（direct EasyNetQ高级API）]]>
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="handler">回调</param>
        /// <param name="exChangeName">交换器名</param>
        /// <param name="queueName">队列名</param>
        /// <param name="routingKey">路由名</param>
        public bool DirectConsume<T>(Action<T> handler, out string msg, string exChangeName = "direct_mq", string queueName = "direct_queue_default", string routingKey = "direct_rout_default") where T : class
        {
            msg = string.Empty;
            try
            {
                var bus = _bus;
                var adbus = bus.Advanced;
                var exchange = adbus.ExchangeDeclare(exChangeName, ExchangeType.Direct);
                var queue = CreateQueue(adbus, queueName);
                adbus.Bind(exchange, queue, routingKey);
                adbus.Consume(queue, registration =>
                {
                    registration.Add<T>((message, info) =>
                    {
                        handler(message.Body);
                    });
                });
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                return false;
            }
            return true;
        }
        #endregion
    }
}

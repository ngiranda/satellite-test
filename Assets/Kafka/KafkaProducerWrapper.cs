using Confluent.Kafka;
using Google.Protobuf;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assets.Kafka
{
    class KafkaProducerWrapper
    {
        private IProducer<string, byte[]> producer;
        private string clientId;
        private BlockingCollection<Tuple<string, Message<string, byte[]>>> messageQueue;
        public KafkaProducerWrapper(string clientId, string host)
        {
            ProducerConfig producerConfig = new ProducerConfig
            {
                BootstrapServers = host,
                ClientId = clientId,
            };
            this.clientId = clientId;
            IProducer<string, byte[]> producer = new ProducerBuilder<string, byte[]>(producerConfig).
                SetValueSerializer(Serializers.ByteArray).
                Build();
            this.producer = producer;
            Thread producerThread = new Thread(() => startProducerThread(producer));
        }

        public void sendMessage(string topic, IMessage protoMessage)
        {
            Message<string, byte[]> message = createKafkaMessage(protoMessage);
            producer.Produce(topic, message);
            producer.Flush();
        }

        private Message<string, byte[]> createKafkaMessage(IMessage protoMessage)
        {
            Message<string, byte[]> message = new Message<string, byte[]>();
            Header header = new Header("ClientId", Encoding.ASCII.GetBytes(clientId));
            Headers headers = new Headers();
            headers.Add(header);
            message.Headers = headers;

            message.Key = "default-key";
            message.Value = protoMessage.ToByteArray();
            return message;
        }
        

        public void addMessageToQueue(string topic, IMessage protoMessage)
        {
            messageQueue.Add(new Tuple<string, Message<string, byte[]>>(topic, createKafkaMessage(protoMessage)));
        }

        private void startProducerThread(IProducer<string, byte[]> producer)
        {
            using(producer)
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true; //prevent the process from terminmating
                    cts.Cancel();
                };
                try
                {
                    while (true)
                    {
                        Tuple<string, Message<string, byte[]>> topicAndMessage = messageQueue.Take();
                        producer.Produce(topicAndMessage.Item1, topicAndMessage.Item2);
                    }
                } catch (OperationCanceledException)
                {
                    producer.Flush();
                }

            }
        }
    }
}

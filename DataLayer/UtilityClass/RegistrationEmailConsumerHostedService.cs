using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MimeKit;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Newtonsoft.Json;

namespace DataLayer.UtilityClass
{
    public class RegistrationEmailConsumerHostedService : BackgroundService
    {
        private readonly IConfiguration _configuration;

        public RegistrationEmailConsumerHostedService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQ:Host"],
                Port = int.Parse(_configuration["RabbitMQ:Port"]),
                UserName = _configuration["RabbitMQ:Username"],
                Password = _configuration["RabbitMQ:Password"]
            };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "user_registration_queue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var registrationInfo = JsonConvert.DeserializeObject<RegistrationMessage>(message);

                await SendRegistrationEmail(registrationInfo.Email, registrationInfo.Name);
            };

            channel.BasicConsume(queue: "user_registration_queue",
                                 autoAck: true,
                                 consumer: consumer);

            return Task.CompletedTask; // Keeps the background task running
        }

        private async Task SendRegistrationEmail(string email, string name)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Fundoo", "vaibhav.koli30@gmail.com"));
            emailMessage.To.Add(new MailboxAddress(name, email));
            emailMessage.Subject = "Welcome to Our Application!";
            emailMessage.Body = new TextPart("plain")
            {
                Text = $"Hi {name},\n\nWelcome to our application!"
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_configuration["SMTP:Host"], int.Parse(_configuration["SMTP:Port"]), SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_configuration["SMTP:Username"], _configuration["SMTP:Password"]);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }

        private class RegistrationMessage
        {
            public string Email { get; set; }
            public string Name { get; set; }
        }
    }
}

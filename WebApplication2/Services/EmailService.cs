using Isa2017Cinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace WebApplication2.Services
{
    public class EmailService
    {
        public EmailService()
        {

        }

        public void SendConfirmationEmail(ApplicationUser user, string url)
        {

          /*  SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 25);
            NetworkCredential basicCredential =
                    new NetworkCredential("conferenceroom40@gmail.com", "scheduler2017");
            MailAddress fromAddress = new MailAddress("conferenceroom40@gmail.com");

            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = basicCredential;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;

            MailMessage message = new MailMessage(fromAddress.ToString(), user.Email);
            message.From = fromAddress;
            message.Subject = "Confirm your account on ConferenceRoomScheduler";
            message.IsBodyHtml = true;
            message.Body = "<h1>You are successfully registered to Conference Room Scheduler!</h1> <h3> Your info: </h3> <br/> <p>User name: " + user.UserName + "</p> <br/> <p> Password:" + user.Password + "<p>"
                + "<br/><p> <strong> Please confirm your account by clicking <a href =\""
                  + url + "\">here</a></strong> <br/><br/> Conference Room Scheduler </p>";
            smtpClient.Send(message);*/
        }
        public void SendInvitationEmail(ApplicationUser invitedUser, ApplicationUser inviter, Ticket reservation, string callbackUrl, string callbackUrl1)
        {

            var fromAddress = new MailAddress("isaNS2017@gmail.com", "ISA NS");
            var toAddress = new MailAddress(invitedUser.Email, "ISA NS");
            string fromPassword = "isa2017_123";
            string subject = "Invitation";
            int red = reservation.SeatRow + 1;
            int kolona = reservation.SeatColumn + 1;
            string body = "You are invited to projection:" + System.Environment.NewLine + "Event name: " + reservation.Projection.Projection.Name + System.Environment.NewLine + " Event is starting from: " + reservation.Projection.Time + System.Environment.NewLine +
                " at:  " + reservation.Projection.Hall.ParentLocation.Name + "," + reservation.Projection.Hall.ParentLocation.Address + "." + System.Environment.NewLine  + "Seat(row-column):" + red +"-" + kolona + System.Environment.NewLine + "User: " + inviter.Name + " " + inviter.LastName + " called you." 
               +System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine + "Click on this link to accept invitation: " + callbackUrl + System.Environment.NewLine + System.Environment.NewLine + 
               "Click on this link to decline invitation: " + callbackUrl1;
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
        public void SendNotificationEmail(ApplicationUser invitedUser, ApplicationUser inviter, Ticket reservation, bool confirmedReservation)
        {
            var fromAddress = new MailAddress("isaNS2017@gmail.com", "ISA NS");
            var toAddress = new MailAddress(inviter.Email, "ISA NS");
            string fromPassword = "isa2017_123";
            string subject = "Notification";
            string body = "";
            if (confirmedReservation)
            {
                body = "User :" + invitedUser.Name + "," + invitedUser.LastName + " accepted your invitation to: " + System.Environment.NewLine + "Event name: " + reservation.Projection.Projection.Name + System.Environment.NewLine + " Event is starting from: " + reservation.Projection.Time + System.Environment.NewLine +
                    " at:  " + reservation.Projection.Hall.ParentLocation.Name + "," + reservation.Projection.Hall.ParentLocation.Address + ".";
            }else
            {
                body = "User :" + invitedUser.Name + "," + invitedUser.LastName + " declined your invitation to: " + System.Environment.NewLine + "Event name: " + reservation.Projection.Projection.Name + System.Environment.NewLine + " Event is starting from: " + reservation.Projection.Time + System.Environment.NewLine +
                   " at:  " + reservation.Projection.Hall.ParentLocation.Name + "," + reservation.Projection.Hall.ParentLocation.Address + ".";
            }
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }

    }
}
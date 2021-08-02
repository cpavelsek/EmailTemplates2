using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmailTemplates2.Helpers;
using EmailTemplates2.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit;
using MimeKit.Text;

namespace EmailTemplates2.Views.Home
{
    public class WelcomeModel : PageModel
    {
        private readonly ContactViewModel _context;

        public WelcomeModel(ContactViewModel context) {
            _context = context;
		}

        [BindProperty]
        public ContactViewModel contactViewModel { get; set; }

        public IActionResult Contact( ContactViewModel contactViewModel ) {
            if ( ModelState.IsValid ) {
                try {
                    string emailBody = string.Empty;
                    bool useEmailTemplate = true;
                    //instantiate a new MimeMessage
                    var message = new MimeMessage();

                    //Setting the To e-mail address
                    message.To.Add( new MailboxAddress( contactViewModel.Name, contactViewModel.Email ) );
                    //Setting the From e-mail address
                    message.From.Add( new MailboxAddress( "Christopher", "thebigdog905@gmail.com" ) );
                    //E-mail subject 
                    message.Subject = contactViewModel.Subject;
                    //E-mail message body
                    if ( useEmailTemplate ) {
                       // emailBody = ViewsToStringOutputHelper.RenderRazorViewToString( this, "Welcome", contactViewModel );
                    } else {
                        emailBody = contactViewModel.Message + " Message was sent by: " + contactViewModel.Name + " E-mail: " + contactViewModel.Email;

                    }
                    message.Body = new TextPart( TextFormat.Html ) {
                        Text = emailBody
                    };

                    //Configure the e-mail
                    using ( var emailClient = new SmtpClient() ) {
                        emailClient.Connect( "smtp.gmail.com", 587, false );
                        emailClient.Authenticate( "thebigdog905@gmail.com", "sparten117" );
                        emailClient.Send( message );
                        emailClient.Disconnect( true );
                    }
                }
                catch ( Exception ex ) {

                    ModelState.Clear();
                    //ViewBag.Exception = $" Oops! Message could not be sent. Error:  {ex.Message}";
                }

            }
            return Redirect( "Index" );
        }
    }
}

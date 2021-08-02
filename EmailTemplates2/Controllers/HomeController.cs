using EmailTemplates2.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EmailTemplates2.Helpers;
using Microsoft.AspNetCore.Authorization;
using static EmailTemplates2.Startup;

namespace EmailTemplates2.Controllers {
	public class HomeController : Controller {
		private readonly ILogger<HomeController> _logger;

		public HomeController( ILogger<HomeController> logger ) {
			_logger = logger;
		}

		public IActionResult Index() {
			return View();
		}

		public IActionResult Privacy() {
			return View();
		}

		[ResponseCache( Duration = 0, Location = ResponseCacheLocation.None, NoStore = true )]
		public IActionResult Error() {
			return View( new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier } );
		}

        public IActionResult Contact() {
            ViewData[ "Message" ] = "Your contact page.";

            return View();
        }

        [HttpPost]
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
                    message.From.Add( new MailboxAddress( "Christopher", "chris.p@inet-web.com" ) );
                    //E-mail subject 
                    message.Subject = contactViewModel.Subject;
                    //E-mail message body
                    if ( useEmailTemplate ) {
                        emailBody = ViewsToStringOutputHelper.RenderRazorViewToString( this, "Welcome", contactViewModel );
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
                    ViewBag.Exception = $" Oops! Message could not be sent. Error:  {ex.Message}";
                }

            }
            return View("Index");
        }

        [NoDirectAccess]
        public IActionResult Welcome() {
            return View();
        }
    }
 
}

using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text.Json;
using System.Threading;

namespace InvictaInternalAPI.Controllers
{
    public static class GmailSupport
    {
        public static void SendEmail(string prefix, string subject, string emailTo, string msgHtml, List<string> files)
        {

            string[] Scopes = { GmailService.Scope.GmailReadonly, GmailService.Scope.GmailModify };

            UserCredential credential;

            string ApplicationName = "InvictaPortal";

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                //Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Gmail API service.
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            MailMessage mail = new MailMessage();
            mail.Subject = subject;
            mail.Body = msgHtml;
            mail.From = new MailAddress("notifications@invictastores.com");
            mail.IsBodyHtml = true;

            foreach (string fileName in files)
            {
                mail.Attachments.Add(new System.Net.Mail.Attachment(".\\Download\\" + fileName));
            }

            mail.To.Add(new MailAddress(emailTo));
            mail.Bcc.Add("help@invictastores.com");
            MimeMessage mimeMessage = MimeMessage.CreateFromMailMessage(mail);
            Message message = new Message();
            message.Raw = Base64UrlEncode(mimeMessage.ToString());
            var response = service.Users.Messages.Send(message, "me").Execute();
        }

        private static string Base64UrlEncode(string input)
        {
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            // Special "url-safe" base64 encode.
            return Convert.ToBase64String(inputBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .Replace("=", "");
        }

        public static void SendCreditMemoEmail(string prefix, string reason, JsonDocument creditMemoObj, JsonDocument orderObj,double shipping_amount)
        {

            string htmlMsg = "";
            string[] lines;

            if (prefix.Equals("TCO"))
            {
                lines = System.IO.File.ReadAllLines(@".\controllers\templates\TCO\creditmemotemplate1.html");
            }
            else
            {
                lines = System.IO.File.ReadAllLines(@".\controllers\templates\Invicta\creditmemotemplate1.html");
            }
            foreach (string line in lines)
            {
                htmlMsg = htmlMsg + line;
            }

            htmlMsg = htmlMsg + "<h3 style=\"font-family:Verdana,Arial;font-weight:normal;font-size:17px;margin-bottom:10px;margin-top:15px\">Your <span class=\"il\">Credit</span> <span class=\"il\">Memo</span> <span>#";
            htmlMsg = htmlMsg + creditMemoObj.RootElement.GetProperty("items")[0].GetProperty("increment_id").GetString() + "</span></h3>";


            string[] reasonlines;
            try
            {
                //reasonlines = System.IO.File.ReadAllLines(@".\controllers\templates\" + reason.ToUpper() + ".html");
                reasonlines = System.IO.File.ReadAllLines(@".\controllers\templates\OUT OF STOCK.html");

            }
            catch
            {
                reasonlines = System.IO.File.ReadAllLines(@".\controllers\templates\DEFAULT.html");
            }

            foreach (string line in reasonlines)
            {
                htmlMsg = htmlMsg + line;
            }

            string[] lines2;
            if (prefix.Equals("TCO"))
            {
                lines2 = System.IO.File.ReadAllLines(@".\controllers\templates\TCO\creditmemotemplate2.html");
            }
            else
            {
                lines2 = System.IO.File.ReadAllLines(@".\controllers\templates\Invicta\creditmemotemplate2.html");
            }
            foreach (string line in lines2)
            {
                htmlMsg = htmlMsg + line;
            }

            htmlMsg = htmlMsg + "<tbody bgcolor=\"#F6F6F6\">";
            foreach (var row in creditMemoObj.RootElement.GetProperty("items")[0].GetProperty("items").EnumerateArray())
            {
                htmlMsg = htmlMsg + "<tr>";
                htmlMsg = htmlMsg + "<td align=\"left\" valign=\"top\" style=\"padding:3px 9px;border-bottom:1px dotted #cccccc;font-family:Verdana,Arial;font-weight:ormal;border-collapse:collapse;vertical-align:top;margin:0\"><strong style=\"font-size:11px;font-family:Verdana,Arial;font-weight:normal\">";
                htmlMsg = htmlMsg + row.GetProperty("name").GetString() + "</strong></td>";
                htmlMsg = htmlMsg + "<td align=\"left\" valign=\"top\" style=\"font-size:11px;padding:3px 9px;border-bottom:1px dotted #cccccc;font-family:Verdana,Arial;font-weight:normal;border-collapse:collapse;vertical-align:top;margin:0\">";
                htmlMsg = htmlMsg + row.GetProperty("sku").GetString() + "</td>";
                htmlMsg = htmlMsg + "<td align=\"center\" valign=\"top\" style=\"font-size:11px;padding:3px 9px;border-bottom:1px dotted #cccccc;font-family:Verdana,Arial;font-weight:normal;border-collapse:collapse;vertical-align:top;margin:0\">";
                htmlMsg = htmlMsg + row.GetProperty("qty").GetInt32().ToString() + "</td>";
                htmlMsg = htmlMsg + "<td align=\"right\" valign=\"top\" style=\"font-size:11px;padding:3px 9px;border-bottom:1px dotted #cccccc;font-family:Verdana,Arial;font-weight:normal;border-collapse:collapse;vertical-align:top;margin:0\"><span style=\"font-family:&quot;Helvetica Neue&quot;,Verdana,Arial,sans-serif\">";
                var subtotal = row.GetProperty("qty").GetInt32() * row.GetProperty("price").GetDecimal();
                htmlMsg = htmlMsg + subtotal.ToString() + "</span></td>";
            }
            htmlMsg = htmlMsg + "</tbody>";

            htmlMsg = htmlMsg + "<tfoot> <tr>";
            htmlMsg = htmlMsg + "<td colspan=\"3\" align=\"right\" style=\"padding:3px 9px;font-family:Verdana,Arial;font-weight:normal;border-collapse:collapse;vertical-align:top;margin:0\">Subtotal</td>";
            htmlMsg = htmlMsg + "<td align=\"right\" style=\"padding:3px 9px;font-family:Verdana,Arial;font-weight:normal;border-collapse:collapse;vertical-align:top;margin:0\"><span style=\"font-family:&quot;Helvetica Neue&quot;,Verdana,Arial,sans-serif\">";
            htmlMsg = htmlMsg + creditMemoObj.RootElement.GetProperty("items")[0].GetProperty("subtotal").GetDecimal().ToString() + "</span></td></tr>";
            
            //Shipping Amount
            if (shipping_amount != 0)
            {
                htmlMsg = htmlMsg + "<tr><td colspan=\"3\" align=\"right\" style=\"padding:3px 9px;font-family:Verdana,Arial;font-weight:normal;border-collapse:collapse;vertical-align:top;margin:0\"><strong style=\"font-family:Verdana,Arial;font-weight:normal\">Shipping & Handling</strong></td>";
                htmlMsg = htmlMsg + "<td align=\"right\" style=\"padding:3px 9px;font-family:Verdana,Arial;font-weight:normal;border-collapse:collapse;vertical-align:top;margin:0\"><span style=\"font-family:&quot;Helvetica Neue&quot;,Verdana,Arial,sans-serif\">";
                htmlMsg = htmlMsg + creditMemoObj.RootElement.GetProperty("items")[0].GetProperty("shipping_amount").GetDecimal().ToString() + "</span></td></tr>";
                htmlMsg = htmlMsg + "<tr><td colspan=\"3\" align=\"right\" style=\"padding:3px 9px;font-family:Verdana,Arial;font-weight:normal;border-collapse:collapse;vertical-align:top;margin:0\"><strong style=\"font-family:Verdana,Arial;font-weight:normal\">Tax</strong></td>";
                htmlMsg = htmlMsg + "<td align=\"right\" style=\"padding:3px 9px;font-family:Verdana,Arial;font-weight:normal;border-collapse:collapse;vertical-align:top;margin:0\"><span style=\"font-family:&quot;Helvetica Neue&quot;,Verdana,Arial,sans-serif\">";
                htmlMsg = htmlMsg + ((creditMemoObj.RootElement.GetProperty("items")[0].GetProperty("tax_amount").GetDecimal()) + creditMemoObj.RootElement.GetProperty("items")[0].GetProperty("shipping_tax_amount").GetDecimal()).ToString() + "</span></td></tr>";
                htmlMsg = htmlMsg + "<tr><td colspan=\"3\" align=\"right\" style=\"padding:3px 9px;font-family:Verdana,Arial;font-weight:normal;border-collapse:collapse;vertical-align:top;margin:0\"><strong style=\"font-family:Verdana,Arial;font-weight:normal\">Grand Total (Incl.Tax)</strong></td>";
                htmlMsg = htmlMsg + "<td align=\"right\" style=\"padding:3px 9px;font-family:Verdana,Arial;font-weight:normal;border-collapse:collapse;vertical-align:top;margin:0\"><span style=\"font-family:&quot;Helvetica Neue&quot;,Verdana,Arial,sans-serif\">";
                htmlMsg = htmlMsg + ((creditMemoObj.RootElement.GetProperty("items")[0].GetProperty("subtotal_incl_tax").GetDecimal()) + (creditMemoObj.RootElement.GetProperty("items")[0].GetProperty("shipping_amount").GetDecimal()) + (creditMemoObj.RootElement.GetProperty("items")[0].GetProperty("shipping_tax_amount").GetDecimal())).ToString() + "</span></td></tr>";
            }
            else
            {
                htmlMsg = htmlMsg + "<tr><td colspan=\"3\" align=\"right\" style=\"padding:3px 9px;font-family:Verdana,Arial;font-weight:normal;border-collapse:collapse;vertical-align:top;margin:0\"><strong style=\"font-family:Verdana,Arial;font-weight:normal\">Grand Total (Excl.Tax)</strong></td>";
                htmlMsg = htmlMsg + "<td align=\"right\" style=\"padding:3px 9px;font-family:Verdana,Arial;font-weight:normal;border-collapse:collapse;vertical-align:top;margin:0\"><span style=\"font-family:&quot;Helvetica Neue&quot;,Verdana,Arial,sans-serif\">";
                htmlMsg = htmlMsg + creditMemoObj.RootElement.GetProperty("items")[0].GetProperty("subtotal").GetDecimal().ToString() + "</span></td></tr>";
                htmlMsg = htmlMsg + "<tr><td colspan=\"3\" align=\"right\" style=\"padding:3px 9px;font-family:Verdana,Arial;font-weight:normal;border-collapse:collapse;vertical-align:top;margin:0\"><strong style=\"font-family:Verdana,Arial;font-weight:normal\">Tax</strong></td>";
                htmlMsg = htmlMsg + "<td align=\"right\" style=\"padding:3px 9px;font-family:Verdana,Arial;font-weight:normal;border-collapse:collapse;vertical-align:top;margin:0\"><span style=\"font-family:&quot;Helvetica Neue&quot;,Verdana,Arial,sans-serif\">";
                htmlMsg = htmlMsg + creditMemoObj.RootElement.GetProperty("items")[0].GetProperty("tax_amount").GetDecimal().ToString() + "</span></td></tr>";
                htmlMsg = htmlMsg + "<tr><td colspan=\"3\" align=\"right\" style=\"padding:3px 9px;font-family:Verdana,Arial;font-weight:normal;border-collapse:collapse;vertical-align:top;margin:0\"><strong style=\"font-family:Verdana,Arial;font-weight:normal\">Grand Total (Incl.Tax)</strong></td>";
                htmlMsg = htmlMsg + "<td align=\"right\" style=\"padding:3px 9px;font-family:Verdana,Arial;font-weight:normal;border-collapse:collapse;vertical-align:top;margin:0\"><span style=\"font-family:&quot;Helvetica Neue&quot;,Verdana,Arial,sans-serif\">";
                htmlMsg = htmlMsg + creditMemoObj.RootElement.GetProperty("items")[0].GetProperty("subtotal_incl_tax").GetDecimal().ToString() + "</span></td></tr>";
            }

            htmlMsg = htmlMsg + "<tr></tfood>";
            htmlMsg = htmlMsg + "</table>";
            htmlMsg = htmlMsg + "<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\" style=\"border-collapse:collapse;padding:0;margin:0;width:100%\">";
            htmlMsg = htmlMsg + "<tbody><tr>";
            htmlMsg = htmlMsg + "<td style=\"font-family:Verdana,Arial;font-weight:normal;border-collapse:collapse;vertical-align:top;padding:10px 15px 0;margin:0;padding-top:10px;text-align:left\"><h6 style=\"font-family:Verdana,Arial;font-weight:700;font-size:12px;margin-bottom:0px;margin-top:5px;text-transform:uppercase\">Bill to:</h6><p style=\"font-family:Verdana,Arial;font-weight:normal;font-size:12px;line-height:18px;margin-bottom:15px;margin-top:2px\"><span>";
            htmlMsg = htmlMsg + orderObj.RootElement.GetProperty("items")[0].GetProperty("billing_address").GetProperty("firstname").GetString() + "&nbsp;";
            htmlMsg = htmlMsg + orderObj.RootElement.GetProperty("items")[0].GetProperty("billing_address").GetProperty("lastname").GetString() + "<br>";
            htmlMsg = htmlMsg + orderObj.RootElement.GetProperty("items")[0].GetProperty("billing_address").GetProperty("street")[0].GetString() + "<br>";
            htmlMsg = htmlMsg + orderObj.RootElement.GetProperty("items")[0].GetProperty("billing_address").GetProperty("city").GetString() + ", ";
            htmlMsg = htmlMsg + orderObj.RootElement.GetProperty("items")[0].GetProperty("billing_address").GetProperty("region").GetString() + ", ";
            htmlMsg = htmlMsg + orderObj.RootElement.GetProperty("items")[0].GetProperty("billing_address").GetProperty("postcode").GetString() + "<br>";
            htmlMsg = htmlMsg + orderObj.RootElement.GetProperty("items")[0].GetProperty("billing_address").GetProperty("country_id").GetString() + "<br>";
            htmlMsg = htmlMsg + "T: " + orderObj.RootElement.GetProperty("items")[0].GetProperty("billing_address").GetProperty("telephone").GetString() + "<br>";
            htmlMsg = htmlMsg + "</span></p></td>";
            htmlMsg = htmlMsg + "<td style=\"font-family:Verdana,Arial;font-weight:normal;border-collapse:collapse;vertical-align:top;padding:10px 15px 0;margin:0;padding-top:10px;text-align:left\"><h6 style=\"font-family:Verdana,Arial;font-weight:700;font-size:12px;margin-bottom:0px;margin-top:5px;text-transform:uppercase\">Ship to:</h6><p style=\"font-family:Verdana,Arial;font-weight:normal;font-size:12px;line-height:18px;margin-bottom:15px;margin-top:2px\"><span>";
            htmlMsg = htmlMsg + orderObj.RootElement.GetProperty("items")[0].GetProperty("extension_attributes").GetProperty("shipping_assignments")[0].GetProperty("shipping").GetProperty("address").GetProperty("firstname").GetString() + "&nbsp;";
            htmlMsg = htmlMsg + orderObj.RootElement.GetProperty("items")[0].GetProperty("extension_attributes").GetProperty("shipping_assignments")[0].GetProperty("shipping").GetProperty("address").GetProperty("lastname").GetString() + "<br>";
            htmlMsg = htmlMsg + orderObj.RootElement.GetProperty("items")[0].GetProperty("extension_attributes").GetProperty("shipping_assignments")[0].GetProperty("shipping").GetProperty("address").GetProperty("street")[0].GetString() + "<br>";
            htmlMsg = htmlMsg + orderObj.RootElement.GetProperty("items")[0].GetProperty("extension_attributes").GetProperty("shipping_assignments")[0].GetProperty("shipping").GetProperty("address").GetProperty("city").GetString() + ", ";
            htmlMsg = htmlMsg + orderObj.RootElement.GetProperty("items")[0].GetProperty("extension_attributes").GetProperty("shipping_assignments")[0].GetProperty("shipping").GetProperty("address").GetProperty("region").GetString() + ", ";
            htmlMsg = htmlMsg + orderObj.RootElement.GetProperty("items")[0].GetProperty("extension_attributes").GetProperty("shipping_assignments")[0].GetProperty("shipping").GetProperty("address").GetProperty("postcode").GetString() + "<br>";
            htmlMsg = htmlMsg + orderObj.RootElement.GetProperty("items")[0].GetProperty("extension_attributes").GetProperty("shipping_assignments")[0].GetProperty("shipping").GetProperty("address").GetProperty("country_id").GetString() + "<br>";
            htmlMsg = htmlMsg + "T: " + orderObj.RootElement.GetProperty("items")[0].GetProperty("extension_attributes").GetProperty("shipping_assignments")[0].GetProperty("shipping").GetProperty("address").GetProperty("telephone").GetString() + "<br>";
            htmlMsg = htmlMsg + "</span></p></td></tr>";
            htmlMsg = htmlMsg + "<tr>";
            htmlMsg = htmlMsg + "<td style=\"font-family:Verdana,Arial;font-weight:normal;border-collapse:collapse;vertical-align:top;padding:10px 15px 0;margin:0;text-align:left;padding-bottom:10px\">";
            htmlMsg = htmlMsg + "<h6 style=\"font-family:Verdana,Arial;font-weight:700;text-align:left;font-size:12px;margin-bottom:0px;margin-top:5px;text-transform:uppercase\">Shipping method:</h6>";
            htmlMsg = htmlMsg + "<p style=\"font-family:Verdana,Arial;font-weight:normal;text-align:left;font-size:12px;margin-top:2px;margin-bottom:30px;line-height:18px;padding:0\">";
            htmlMsg = htmlMsg + orderObj.RootElement.GetProperty("items")[0].GetProperty("shipping_description").GetString();
            htmlMsg = htmlMsg + "</p></td>";
            htmlMsg = htmlMsg + "<td style=\"font-family:Verdana,Arial;font-weight:normal;border-collapse:collapse;vertical-align:top;padding:10px 15px 0;margin:0;text-align:left;padding-bottom:10px\">";
            htmlMsg = htmlMsg + "<h6 style=\"font-family:Verdana,Arial;font-weight:700;text-align:left;font-size:12px;margin-bottom:0px;margin-top:5px;text-transform:uppercase\">Payment method::</h6>";
            htmlMsg = htmlMsg + "<p style=\"font-family:Verdana,Arial;font-weight:normal;text-align:left;font-size:12px;margin-top:2px;margin-bottom:30px;line-height:18px;padding:0\">";

            foreach (JsonElement elm in orderObj.RootElement.GetProperty("items")[0].GetProperty("extension_attributes").GetProperty("payment_additional_info").EnumerateArray())
            {
                Console.WriteLine("elm:" + elm.ToString());
                if (elm.GetProperty("key").GetString().Equals("method_title"))
                {
                    htmlMsg = htmlMsg + elm.GetProperty("value").GetString();
                }
            }
            htmlMsg = htmlMsg + "</p><td></tr>";
            htmlMsg = htmlMsg + "</tbody></table></td> </tr></tbody></table></td></tr></tbody></table>";
            //htmlMsg = htmlMsg + "<h5 style=\"font-family:Verdana,Arial;font-weight:normal;text-align:center;font-size:22px;line-height:32px;margin-bottom:75px;margin-top:30px\">Thank you, InvictaStores.com!</h5>";


            var subject = "";
            if (prefix.Equals("TCO"))
            {
                htmlMsg = htmlMsg + "<h5 style=\"font-family:Verdana,Arial;font-weight:normal;text-align:center;font-size:22px;line-height:32px;margin-bottom:75px;margin-top:30px\">Thank you, thecloseout.com!</h5>";

                subject = "thecloseout.com: Regarding Your Order # " + orderObj.RootElement.GetProperty("items")[0].GetProperty("increment_id").GetString();
            }
            else
            {
                htmlMsg = htmlMsg + "<h5 style=\"font-family:Verdana,Arial;font-weight:normal;text-align:center;font-size:22px;line-height:32px;margin-bottom:75px;margin-top:30px\">Thank you, InvictaStores.com!</h5>";

                subject = "InvictaStores.com: Regarding Your Order # " + orderObj.RootElement.GetProperty("items")[0].GetProperty("increment_id").GetString();
            }
            var email = orderObj.RootElement.GetProperty("items")[0].GetProperty("customer_email").GetString();

            SendEmail(prefix, subject, email, htmlMsg, new List<string>());

        }
    }
}

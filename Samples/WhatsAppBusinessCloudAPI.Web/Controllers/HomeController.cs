﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using WhatsappBusiness.CloudApi;
using WhatsappBusiness.CloudApi.Configurations;
using WhatsappBusiness.CloudApi.Exceptions;
using WhatsappBusiness.CloudApi.Interfaces;
using WhatsappBusiness.CloudApi.Media.Requests;
using WhatsappBusiness.CloudApi.Messages.Requests;
using WhatsappBusiness.CloudApi.Response;
using WhatsAppBusinessCloudAPI.Web.Models;
using WhatsAppBusinessCloudAPI.Web.ViewModel;

namespace WhatsAppBusinessCloudAPI.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWhatsAppBusinessClient _whatsAppBusinessClient;
        private readonly WhatsAppBusinessCloudApiConfig _whatsAppConfig;
        private readonly IWebHostEnvironment _environment;

        public HomeController(ILogger<HomeController> logger, IWhatsAppBusinessClient whatsAppBusinessClient,
            IOptions<WhatsAppBusinessCloudApiConfig> whatsAppConfig, IWebHostEnvironment environment)
        {
            _logger = logger;
            _whatsAppBusinessClient = whatsAppBusinessClient;
            _whatsAppConfig = whatsAppConfig.Value;
            _environment = environment;
        }

        public IActionResult Index()
        {
            TempData["Success"] = null;
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult SendText()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendText(SendTextMessageViewModel sendTextMessageViewModel)
        {
            try
            {
                TextMessageRequest textMessageRequest = new TextMessageRequest();
                textMessageRequest.To = sendTextMessageViewModel.RecipientPhoneNumber;
                textMessageRequest.Text = new WhatsAppText();
                textMessageRequest.Text.Body = sendTextMessageViewModel.Message;
                textMessageRequest.Text.PreviewUrl = false;

                var results = await _whatsAppBusinessClient.SendTextMessageAsync(textMessageRequest);
                TempData["Success"] = "Sent Successfully!";
                return View();
            }
            catch (WhatsappBusinessCloudAPIException ex)
            {
                _logger.LogError(ex, ex.Message);
                return View();
            }
        }

        public IActionResult SendMedia()
        {
            SendMediaMessageViewModel sendMediaMessageViewModel = new SendMediaMessageViewModel();
            sendMediaMessageViewModel.MediaType = new List<SelectListItem>()
            {
                new SelectListItem(){ Text = "Audio", Value = "Audio" },
                new SelectListItem(){ Text = "Document", Value = "Document" },
                new SelectListItem(){ Text = "Image", Value = "Image" },
                new SelectListItem(){ Text = "Sticker", Value = "Sticker" },
                new SelectListItem(){ Text = "Video", Value = "Video" },
            };
           
            return View(sendMediaMessageViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMedia(SendMediaMessageViewModel sendMediaMessage)
        {
            try
            {
                WhatsAppResponse results = null;
                switch(sendMediaMessage.SelectedMediaType)
                {
                    case "Audio":
                        if (!string.IsNullOrWhiteSpace(sendMediaMessage.MediaId))
                        {
                            AudioMessageByIdRequest audioMessage = new AudioMessageByIdRequest();
                            audioMessage.To = sendMediaMessage.RecipientPhoneNumber;
                            audioMessage.Audio = new MediaAudio();
                            audioMessage.Audio.Id = sendMediaMessage.MediaId;

                            results = await _whatsAppBusinessClient.SendAudioAttachmentMessageByIdAsync(audioMessage);
                        }

                        if (!string.IsNullOrWhiteSpace(sendMediaMessage.MediaLink))
                        {
                            AudioMessageByUrlRequest audioMessage = new AudioMessageByUrlRequest();
                            audioMessage.To = sendMediaMessage.RecipientPhoneNumber;
                            audioMessage.Audio = new MediaAudioUrl();
                            audioMessage.Audio.Link = sendMediaMessage.MediaLink;

                            results = await _whatsAppBusinessClient.SendAudioAttachmentMessageByUrlAsync(audioMessage);
                        }
                        break;

                    case "Document":
                        if (!string.IsNullOrWhiteSpace(sendMediaMessage.MediaId))
                        {
                            DocumentMessageByIdRequest documentMessage = new DocumentMessageByIdRequest();
                            documentMessage.To = sendMediaMessage.RecipientPhoneNumber;
                            documentMessage.Document = new MediaDocument();
                            documentMessage.Document.Id = sendMediaMessage.MediaId;
                            documentMessage.Document.Caption = sendMediaMessage.Message;

                            results = await _whatsAppBusinessClient.SendDocumentAttachmentMessageByIdAsync(documentMessage);
                        }

                        if (!string.IsNullOrWhiteSpace(sendMediaMessage.MediaLink))
                        {
                            DocumentMessageByUrlRequest documentMessage = new DocumentMessageByUrlRequest();
                            documentMessage.To = sendMediaMessage.RecipientPhoneNumber;
                            documentMessage.Document = new MediaDocumentUrl();
                            documentMessage.Document.Link = sendMediaMessage.MediaLink;
                            documentMessage.Document.Caption = sendMediaMessage.Message;

                            results = await _whatsAppBusinessClient.SendDocumentAttachmentMessageByUrlAsync(documentMessage);
                        }
                        break;

                    case "Image":
                        if (!string.IsNullOrWhiteSpace(sendMediaMessage.MediaId))
                        {
                            ImageMessageByIdRequest imageMessage = new ImageMessageByIdRequest();
                            imageMessage.To = sendMediaMessage.RecipientPhoneNumber;
                            imageMessage.Image = new MediaImage();
                            imageMessage.Image.Id = sendMediaMessage.MediaId;
                            imageMessage.Image.Caption = sendMediaMessage.Message;

                            results = await _whatsAppBusinessClient.SendImageAttachmentMessageByIdAsync(imageMessage);
                        }

                        if (!string.IsNullOrWhiteSpace(sendMediaMessage.MediaLink))
                        {
                            ImageMessageByUrlRequest imageMessage = new ImageMessageByUrlRequest();
                            imageMessage.To = sendMediaMessage.RecipientPhoneNumber;
                            imageMessage.Image = new MediaImageUrl();
                            imageMessage.Image.Link = sendMediaMessage.MediaLink;
                            imageMessage.Image.Caption = sendMediaMessage.Message;

                            results = await _whatsAppBusinessClient.SendImageAttachmentMessageByUrlAsync(imageMessage);
                        }
                        break;

                    case "Sticker":
                        if (!string.IsNullOrWhiteSpace(sendMediaMessage.MediaId))
                        {
                            StickerMessageByIdRequest stickerMessage = new StickerMessageByIdRequest();
                            stickerMessage.To = sendMediaMessage.RecipientPhoneNumber;
                            stickerMessage.Sticker = new MediaSticker();
                            stickerMessage.Sticker.Id = sendMediaMessage.MediaId;

                            results = await _whatsAppBusinessClient.SendStickerMessageByIdAsync(stickerMessage);
                        }

                        if (!string.IsNullOrWhiteSpace(sendMediaMessage.MediaLink))
                        {
                            StickerMessageByUrlRequest stickerMessage = new StickerMessageByUrlRequest();
                            stickerMessage.To = sendMediaMessage.RecipientPhoneNumber;
                            stickerMessage.Sticker = new MediaStickerUrl();
                            stickerMessage.Sticker.Link = sendMediaMessage.MediaLink;

                            results = await _whatsAppBusinessClient.SendStickerMessageByUrlAsync(stickerMessage);
                        }
                        break;

                    case "Video":
                        if (!string.IsNullOrWhiteSpace(sendMediaMessage.MediaId))
                        {
                            VideoMessageByIdRequest videoMessage = new VideoMessageByIdRequest();
                            videoMessage.To = sendMediaMessage.RecipientPhoneNumber;
                            videoMessage.Video = new MediaVideo();
                            videoMessage.Video.Id = sendMediaMessage.MediaId;
                            videoMessage.Video.Caption = sendMediaMessage.Message;

                            results = await _whatsAppBusinessClient.SendVideoAttachmentMessageByIdAsync(videoMessage);
                        }

                        if (!string.IsNullOrWhiteSpace(sendMediaMessage.MediaLink))
                        {
                            VideoMessageByUrlRequest videoMessage = new VideoMessageByUrlRequest();
                            videoMessage.To = sendMediaMessage.RecipientPhoneNumber;
                            videoMessage.Video = new MediaVideoUrl();
                            videoMessage.Video.Link = sendMediaMessage.MediaLink;
                            videoMessage.Video.Caption = sendMediaMessage.Message;

                            results = await _whatsAppBusinessClient.SendVideoAttachmentMessageByUrlAsync(videoMessage);
                        }
                        break;
                }

                if (results != null)
                {
                    TempData["Success"] = "Sent Successfully!";
                    return RedirectToAction(nameof(SendMedia));
                }
                else
                {
                    return RedirectToAction(nameof(SendMedia));
                }
            }
            catch (WhatsappBusinessCloudAPIException ex)
            {
                _logger.LogError(ex, ex.Message);
                return RedirectToAction(nameof(SendMedia));
            }
        }

        public IActionResult SendLocation()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendLocation(SendLocationMessageViewModel sendLocationMessageViewModel)
        {
            try
            {
                LocationMessageRequest locationMessageRequest = new LocationMessageRequest();
                locationMessageRequest.To = sendLocationMessageViewModel.RecipientPhoneNumber;
                locationMessageRequest.Location = new Location();
                locationMessageRequest.Location.Name = "Location Test";
                locationMessageRequest.Location.Address = "Address Test";
                locationMessageRequest.Location.Longitude = sendLocationMessageViewModel.Longitude;
                locationMessageRequest.Location.Latitude = sendLocationMessageViewModel.Latitude;

                var results = await _whatsAppBusinessClient.SendLocationMessageAsync(locationMessageRequest);
                TempData["Success"] = "Sent Successfully!";
                return RedirectToAction(nameof(SendLocation));
            }
            catch (WhatsappBusinessCloudAPIException ex)
            {
                _logger.LogError(ex, ex.Message);
                return View();
            }
        }

        public IActionResult SendInteractive()
        {
            SendInteractiveMessageViewModel sendInteractiveMessageViewModel = new SendInteractiveMessageViewModel();
            sendInteractiveMessageViewModel.InteractiveType = new List<SelectListItem>()
            {
                new SelectListItem(){ Text = "List Message", Value = "List Message" },
                new SelectListItem(){ Text = "Reply Button", Value = "Reply Button" }
            };
            return View(sendInteractiveMessageViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendInteractive(SendInteractiveMessageViewModel sendInteractiveMessageViewModel)
        {
            try
            {
                WhatsAppResponse results = null;

                if (sendInteractiveMessageViewModel.SelectedInteractiveType.Equals("List Message"))
                {
                    InteractiveListMessageRequest interactiveListMessage = new InteractiveListMessageRequest();
                    interactiveListMessage.To = sendInteractiveMessageViewModel.RecipientPhoneNumber;
                    interactiveListMessage.Interactive = new InteractiveListMessage();

                    interactiveListMessage.Interactive.Header = new Header();
                    interactiveListMessage.Interactive.Header.Type = "text";
                    interactiveListMessage.Interactive.Header.Text = "List Header Sample Test";

                    interactiveListMessage.Interactive.Body = new ListBody();
                    interactiveListMessage.Interactive.Body.Text = sendInteractiveMessageViewModel.Message;

                    interactiveListMessage.Interactive.Footer = new Footer();
                    interactiveListMessage.Interactive.Footer.Text = "List Footer Sample Test";

                    interactiveListMessage.Interactive.Action = new ListAction();
                    interactiveListMessage.Interactive.Action.Button = "Send";
                    interactiveListMessage.Interactive.Action.Sections = new List<Section>()
                    {
                        new Section()
                        {
                            Title = "Category A",
                            Rows = new List<Row>()
                            {
                                new Row()
                                {
                                    Id = "Item_A1",
                                    Title = "Apples",
                                    Description = "Enjoy fruits for free"
                                },
                                new Row()
                                {
                                    Id = "Item_A2",
                                    Title = "Tangerines",
                                    Description = "Enjoy fruits for free"
                                },
                            },
                        },
                        new Section()
                        {
                            Title = "Category B",
                            Rows = new List<Row>()
                            {
                                new Row()
                                {
                                    Id = "Item_B1",
                                    Title = "2JZ",
                                    Description = "Engine discounts"
                                },
                                new Row()
                                {
                                    Id = "Item_2",
                                    Title = "1JZ",
                                    Description = "Engine discounts"
                                },
                            }
                        }
                    };

                    results = await _whatsAppBusinessClient.SendInteractiveListMessageAsync(interactiveListMessage);
                }

                if (sendInteractiveMessageViewModel.SelectedInteractiveType.Equals("Reply Button"))
                {
                    InteractiveReplyButtonMessageRequest interactiveReplyButtonMessage = new InteractiveReplyButtonMessageRequest();
                    interactiveReplyButtonMessage.To = sendInteractiveMessageViewModel.RecipientPhoneNumber;
                    interactiveReplyButtonMessage.Interactive = new InteractiveReplyButtonMessage();

                    interactiveReplyButtonMessage.Interactive.Body = new ReplyButtonBody();
                    interactiveReplyButtonMessage.Interactive.Body.Text = sendInteractiveMessageViewModel.Message;

                    interactiveReplyButtonMessage.Interactive.Action = new ReplyButtonAction();
                    interactiveReplyButtonMessage.Interactive.Action.Buttons = new List<ReplyButton>()
                    {
                        new ReplyButton() 
                        {
                            Type = "reply",
                            Reply = new Reply()
                            {
                                Id = "SAMPLE_1_CLICK",
                                Title = "CLICK ME!!!"
                            }
                        },

                        new ReplyButton()
                        {
                            Type = "reply",
                            Reply = new Reply()
                            {
                                Id = "SAMPLE_2_CLICK",
                                Title = "LATER"
                            }
                        }
                    };

                    results = await _whatsAppBusinessClient.SendInteractiveReplyButtonMessageAsync(interactiveReplyButtonMessage);
                }

                if (results != null)
                {
                    TempData["Success"] = "Sent Successfully!";
                    return RedirectToAction(nameof(SendInteractive));
                }
                else
                {
                    return RedirectToAction(nameof(SendInteractive));
                }
            }
            catch (WhatsappBusinessCloudAPIException ex)
            {
                _logger.LogError(ex, ex.Message);
                return RedirectToAction(nameof(SendInteractive));
            }
        }

        public IActionResult SendTemplate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendTemplate(SendTemplateMessageViewModel sendTemplateMessageViewModel)
        {
            try
            {
                TextTemplateMessageRequest textTemplateMessage = new TextTemplateMessageRequest();
                textTemplateMessage.To = sendTemplateMessageViewModel.RecipientPhoneNumber;
                textTemplateMessage.Template = new TextMessageTemplate();
                textTemplateMessage.Template.Name = sendTemplateMessageViewModel.TemplateName;
                textTemplateMessage.Template.Language = new TextMessageLanguage();
                textTemplateMessage.Template.Language.Code = "en_US";

                var results = await _whatsAppBusinessClient.SendTextMessageTemplateAsync(textTemplateMessage);

                if (results != null)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return RedirectToAction(nameof(SendTemplate));
                }
            }
            catch (WhatsappBusinessCloudAPIException ex)
            {
                _logger.LogError(ex, ex.Message);
                return RedirectToAction(nameof(SendTemplate));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendWhatsAppTextTemplateMessageWithParameters(SendTemplateMessageViewModel sendTemplateMessageViewModel)
        {
            try
            {
                // For Text Template message with parameters supported component type is body only
                TextTemplateMessageRequest textTemplateMessage = new TextTemplateMessageRequest();
                textTemplateMessage.To = sendTemplateMessageViewModel.RecipientPhoneNumber;
                textTemplateMessage.Template = new TextMessageTemplate();
                textTemplateMessage.Template.Name = sendTemplateMessageViewModel.TemplateName;
                textTemplateMessage.Template.Language = new TextMessageLanguage();
                textTemplateMessage.Template.Language.Code = LanguageCode.English_US;
                textTemplateMessage.Template.Components = new List<TextMessageComponent>();
                textTemplateMessage.Template.Components.Add(new TextMessageComponent()
                {
                    Type = "body",
                    Parameters = new List<TextMessageParameter>()
                    {
                        new TextMessageParameter()
                        {
                            Type = "text",
                            Text = "Testing Parameter Placeholder Position 1"
                        },
                        new TextMessageParameter()
                        {
                            Type = "text",
                            Text = "Testing Parameter Placeholder Position 2"
                        }
                    }
                });

                var results = await _whatsAppBusinessClient.SendTextMessageTemplateAsync(textTemplateMessage);

                if (results != null)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return RedirectToAction(nameof(SendTemplate));
                }
            }
            catch (WhatsappBusinessCloudAPIException ex)
            {
                _logger.LogError(ex, ex.Message);
                return RedirectToAction(nameof(SendTemplate));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendWhatsAppInteractiveTemplateMessageWithParameters(SendTemplateMessageViewModel sendTemplateMessageViewModel)
        {
            try
            {
                // Tested with facebook predefined template name: sample_issue_resolution
                InteractiveTemplateMessageRequest interactiveTemplateMessage = new InteractiveTemplateMessageRequest();
                interactiveTemplateMessage.To = sendTemplateMessageViewModel.RecipientPhoneNumber;
                interactiveTemplateMessage.Template = new InteractiveMessageTemplate();
                interactiveTemplateMessage.Template.Name = sendTemplateMessageViewModel.TemplateName;
                interactiveTemplateMessage.Template.Language = new InteractiveMessageLanguage();
                interactiveTemplateMessage.Template.Language.Code = LanguageCode.English_US;
                interactiveTemplateMessage.Template.Components = new List<InteractiveMessageComponent>();
                interactiveTemplateMessage.Template.Components.Add(new InteractiveMessageComponent()
                {
                    Type = "body",
                    Parameters = new List<InteractiveMessageParameter>()
                    {
                        new InteractiveMessageParameter()
                        {
                            Type = "text",
                            Text = "Interactive Parameter Placeholder Position 1"
                        }
                    }
                });

                var results = await _whatsAppBusinessClient.SendInteractiveTemplateMessageAsync(interactiveTemplateMessage);

                if (results != null)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return RedirectToAction(nameof(SendTemplate));
                }
            }
            catch (WhatsappBusinessCloudAPIException ex)
            {
                _logger.LogError(ex, ex.Message);
                return RedirectToAction(nameof(SendTemplate));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendWhatsAppMediaTemplateMessageWithParameters(SendTemplateMessageViewModel sendTemplateMessageViewModel)
        {
            try
            {
                // Tested with facebook predefined template name: sample_movie_ticket_confirmation
                ImageTemplateMessageRequest imageTemplateMessage = new ImageTemplateMessageRequest();
                imageTemplateMessage.To = sendTemplateMessageViewModel.RecipientPhoneNumber;
                imageTemplateMessage.Template = new ImageMessageTemplate();
                imageTemplateMessage.Template.Name = sendTemplateMessageViewModel.TemplateName;
                imageTemplateMessage.Template.Language = new ImageMessageLanguage();
                imageTemplateMessage.Template.Language.Code = LanguageCode.English_US;
                imageTemplateMessage.Template.Components = new List<ImageMessageComponent>()
                {
                    new ImageMessageComponent()
                    {
                        Type = "header",
                        Parameters = new List<ImageMessageParameter>()
                        {
                            new ImageMessageParameter()
                            {
                                Type = "image",
                                Image = new Image()
                                {
                                    Link = "https://otakukart.com/wp-content/uploads/2022/03/Upcoming-Marvel-Movies-In-2022-23.jpg"
                                }
                            }
                        },
                    },
                    new ImageMessageComponent()
                    {
                        Type = "body",
                        Parameters = new List<ImageMessageParameter>()
                        {
                            new ImageMessageParameter()
                            {
                                Type = "text",
                                Text = "Movie Testing"
                            },

                            new ImageMessageParameter()
                            {
                                Type = "date_time",
                                DateTime = new ImageTemplateDateTime()
                                {
                                    FallbackValue = DateTime.Now.ToString("dddd d, yyyy"),
                                    DayOfWeek = (int)DateTime.Now.DayOfWeek,
                                    Year = DateTime.Now.Year,
                                    Month = DateTime.Now.Month,
                                    DayOfMonth = DateTime.Now.Day,
                                    Hour = DateTime.Now.Hour,
                                    Minute = DateTime.Now.Minute,
                                    Calendar = "GREGORIAN"
                                }
                            },

                            new ImageMessageParameter()
                            {
                                Type = "text",
                                Text = "Venue Test"
                            },

                            new ImageMessageParameter()
                            {
                                Type = "text",
                                Text = "Seat 1A, 2A, 3A and 4A"
                            }
                        }
                    }
                };

                var results = await _whatsAppBusinessClient.SendImageAttachmentTemplateMessageAsync(imageTemplateMessage);

                if (results != null)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return RedirectToAction(nameof(SendTemplate));
                }
            }
            catch (WhatsappBusinessCloudAPIException ex)
            {
                _logger.LogError(ex, ex.Message);
                return RedirectToAction(nameof(SendTemplate));
            }
        }

        public IActionResult SendContact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendContact(SendContactMessageViewModel sendContactMessageViewModel)
        {
            try
            {
                ContactMessageRequest contactMessageRequest = new ContactMessageRequest();
                contactMessageRequest.To = sendContactMessageViewModel.RecipientPhoneNumber;
                contactMessageRequest.Contacts = new List<ContactData>()
                {
                    new ContactData()
                    {
                        Addresses = new List<Address>()
                        {
                            new Address()
                            {
                                State = "State Test",
                                City = "City Test",
                                Zip = "Zip Test",
                                Country = "Country Test",
                                CountryCode = "Country Code Test",
                                Type = "Home"
                            }
                        },
                        Name = new Name()
                        {
                            FormattedName = "Testing name",
                            FirstName = "FName",
                            LastName = "LName",
                            MiddleName = "MName"
                        }
                    }
                };

                var results = await _whatsAppBusinessClient.SendContactAttachmentMessageAsync(contactMessageRequest);

                if (results != null)
                {
                    TempData["Success"] = "Sent Successfully!";
                    return RedirectToAction(nameof(SendContact));
                }
                else
                {
                    return RedirectToAction(nameof(SendContact));
                }
            }
            catch (WhatsappBusinessCloudAPIException ex)
            {
                _logger.LogError(ex, ex.Message);
                return View();
            }
        }

        public IActionResult UploadMedia()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadMedia(IFormFile mediaFile)
        {
            try
            {
                var fileName = Path.GetFileName(mediaFile.FileName).Trim('"');

                var rootPath = Path.Combine(_environment.WebRootPath, "Application_Files\\MediaUploads\\");

                if (!Directory.Exists(rootPath))
                {
                    Directory.CreateDirectory(rootPath);
                }

                // Get the path of filename
                var filePath = Path.Combine(_environment.WebRootPath, "Application_Files\\MediaUploads\\", fileName);

                // Upload Csv file to the browser
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await mediaFile.CopyToAsync(stream);
                }

                UploadMediaRequest uploadMediaRequest = new UploadMediaRequest();
                uploadMediaRequest.File = filePath;
                uploadMediaRequest.Type = mediaFile.ContentType;

                var uploadMediaResult = await _whatsAppBusinessClient.UploadMediaAsync(uploadMediaRequest);
                ViewBag.MediaId = uploadMediaResult.MediaId;
                TempData["MediaId"] = uploadMediaResult.MediaId;
               
                return RedirectToAction(nameof(SendMedia));
            }
            catch (WhatsappBusinessCloudAPIException ex)
            {
                _logger.LogError(ex, ex.Message);
                return RedirectToAction(nameof(SendMedia));
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
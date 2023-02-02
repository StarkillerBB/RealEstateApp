using RealEstateApp.Models;
using RealEstateApp.Services;
using RealEstateApp.Views;
using System.Windows.Input;

namespace RealEstateApp.ViewModels;

[QueryProperty(nameof(PropertyListItem), "MyPropertyListItem")]
public class PropertyDetailPageViewModel : BaseViewModel
{
    private readonly IPropertyService service;
    public PropertyDetailPageViewModel(IPropertyService service)
    {
        this.service = service;
    }

    Property property;
    public Property Property { get => property; set { SetProperty(ref property, value); } }


    Agent agent;
    public Agent Agent { get => agent; set { SetProperty(ref agent, value); } }


    PropertyListItem propertyListItem;
    public PropertyListItem PropertyListItem
    {
        set
        {
            SetProperty(ref propertyListItem, value);
           
            Property = propertyListItem.Property;
            Agent = service.GetAgents().FirstOrDefault(x => x.Id == Property.AgentId);
        }
    }

    private Command displayPhoneOptionsCommand;
    public ICommand DisplayPhoneOptionsCommand => displayPhoneOptionsCommand ??= new Command(async () => await DisplayPhoneOptions());
    async Task DisplayPhoneOptions()
    {
        string choice = await Shell.Current.DisplayActionSheet(Property.Vendor.Phone, "Cancel", null, "Call", "SMS");
        if (choice == "Call")
            CallPhone();
        else if (choice == "SMS")
            await TextPhone();
    }

    private void CallPhone()
    {
        if (PhoneDialer.Default.IsSupported)
            PhoneDialer.Default.Open(property.Vendor.Phone);
    }

    async Task TextPhone()
    {
        if (Sms.Default.IsComposeSupported)
        {
            string[] recipients = new[] { property.Vendor.Phone };
            string text = $"Hej, {property.Vendor.FirstName}, angående {property.Address}";

            var message = new SmsMessage(text, recipients);

            await Sms.Default.ComposeAsync(message);
        }
    }


    private Command sendMailCommand;
    public ICommand SendMailCommand => sendMailCommand ??= new Command(async () => await SendMail());

    async Task SendMail()
    {
        if (Email.Default.IsComposeSupported)
        {

            string subject = "Property!";
            string body = "This is the property you have looked at.";
            string[] recipients = new[] { Property.Vendor.Email };

            var message = new EmailMessage
            {
                Subject = subject,
                Body = body,
                BodyFormat = EmailBodyFormat.PlainText,
                To = new List<string>(recipients)
            };

            var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var attachmentFilePath = Path.Combine(folder, "property.txt");
            File.WriteAllText(attachmentFilePath, $"{Property.Address}");

            message.Attachments.Add(new EmailAttachment(attachmentFilePath));

            await Email.Default.ComposeAsync(message);
        }
    }

    private Command editPropertyCommand;
    public ICommand EditPropertyCommand => editPropertyCommand ??= new Command(async () => await GotoEditProperty());
    async Task GotoEditProperty()
    {
        await Shell.Current.GoToAsync($"{nameof(AddEditPropertyPage)}", true, new Dictionary<string, object>
        {
            {"MyProperty", property }
        });
    }
}

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

    private Command openBrowserCommand;
    public ICommand OpenBrowserCommand => openBrowserCommand ??= new Command(async () => await BrowserOpen());
    private async Task BrowserOpen()
    {
        try
        {
            Uri uri = new Uri(property.NeighbourhoodUrl);
            BrowserLaunchOptions options = new BrowserLaunchOptions()
            {
                LaunchMode = BrowserLaunchMode.SystemPreferred,
                TitleMode = BrowserTitleMode.Show,
                PreferredToolbarColor = Colors.Violet,
                PreferredControlColor = Colors.Purple
            };

            await Browser.Default.OpenAsync(uri, options);
        }
        catch (Exception ex)
        {
            // An unexpected error occurred. No browser may be installed on the device.
        }
    }

    private Command openFileCommand;
    public ICommand OpenFileCommand => openFileCommand ??= new Command(async () => await OpenFile());
    private async Task OpenFile()
    {
        string popoverTitle = "Read text file";
        string name = "File.txt";
        string file = System.IO.Path.Combine(FileSystem.CacheDirectory, name);

        System.IO.File.WriteAllText(file, "Hello World");

        await Launcher.Default.OpenAsync(new OpenFileRequest(popoverTitle, new ReadOnlyFile(file)));
    }

    private Command shareTextCommand;
    public ICommand ShareTextCommand => shareTextCommand ??= new Command(async () => await ShareText());
    public async Task ShareText()
    {
        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Uri = property.NeighbourhoodUrl,
            Subject = "A property you may be interested in",
            Text = $"{property.Address}, {property.Price}, {property.Beds}",
            Title = "Share Property"
        });
    }

    private Command shareFileCommand;
    public ICommand ShareFileCommand => shareFileCommand ??= new Command(async () => await ShareFile());
    public async Task ShareFile()
    {
        string fn = "Attachment.txt";
        string file = Path.Combine(FileSystem.CacheDirectory, fn);

        File.WriteAllText(file, "Hello World");

        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Share text file",
            File = new ShareFile(file)
        });
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

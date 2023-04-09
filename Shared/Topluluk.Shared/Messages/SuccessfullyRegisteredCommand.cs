namespace Topluluk.Shared.Messages;

// Mesajı mail servis işleyecek.
public class SuccessfullyRegisteredCommand
{
    public string To { get; set; }
    public string FullName { get; set; }
}
using System.ComponentModel.DataAnnotations.Schema;
using Topluluk.Shared.Dtos;

namespace Topluluk.Services.EventAPI.Model.Entity;

public class EventAttendee : AbstractEntity
{
    public string UserId { get; set; }
    public string EventId { get; set; }
}
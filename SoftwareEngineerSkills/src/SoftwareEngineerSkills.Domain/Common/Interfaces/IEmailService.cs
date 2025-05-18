using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace SoftwareEngineerSkills.Domain.Common.Interfaces;

/// <summary>
/// Service for sending emails as part of domain processes
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email
    /// </summary>
    /// <param name="to">Recipient email address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="body">Email body</param>
    /// <param name="isHtml">Whether the body is HTML</param>
    /// <param name="cancellationToken">A token for cancelling the operation</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Sends an email with attachments
    /// </summary>
    /// <param name="to">Recipient email address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="body">Email body</param>
    /// <param name="attachments">Email attachments</param>
    /// <param name="isHtml">Whether the body is HTML</param>
    /// <param name="cancellationToken">A token for cancelling the operation</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task SendEmailWithAttachmentsAsync(string to, string subject, string body, IEnumerable<Attachment> attachments, bool isHtml = false, CancellationToken cancellationToken = default);
}

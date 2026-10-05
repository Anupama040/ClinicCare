using ClinicManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.Controllers;

[Authorize(Roles = "Admin")]
public class EmailTestController : Controller
{
    private readonly IEmailService _emailService;

    public EmailTestController(
        IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(
        string recipientEmail)
    {
        if (string.IsNullOrWhiteSpace(recipientEmail))
        {
            ModelState.AddModelError(
                string.Empty,
                "Recipient email is required.");

            return View("Index");
        }

        await _emailService.SendEmailAsync(
            recipientEmail,
            "ClinicCare test email",
            """
            <h2>ClinicCare Email Test</h2>
            <p>
                This is a test email from the ClinicCare
                clinic management system.
            </p>
            """);

        TempData["SuccessMessage"] =
            "Test email sent successfully.";

        return RedirectToAction(nameof(Index));
    }
}
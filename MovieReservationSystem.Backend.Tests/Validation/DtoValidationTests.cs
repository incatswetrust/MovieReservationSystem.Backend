using System.ComponentModel.DataAnnotations;
using MovieReservationSystem.Backend.Domain;
using MovieReservationSystem.Backend.DTOs.Movie;
using MovieReservationSystem.Backend.DTOs.User;
using Xunit;

namespace MovieReservationSystem.Backend.Tests.Validation;

/// <summary>
/// Confirms the [MaxLength(...)] attributes added this session actually reject over-length
/// input. Uses System.ComponentModel.DataAnnotations.Validator directly against the DTOs —
/// this is the same validation ASP.NET Core's model binding runs under the hood before an
/// action method body executes, so no HTTP pipeline is needed to exercise it.
/// </summary>
public class DtoValidationTests
{
    private static IList<ValidationResult> Validate(object dto)
    {
        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(dto, context, results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public void MovieCreateDto_TitleOverMaxLength_FailsValidation()
    {
        var dto = new MovieCreateDto
        {
            Title = new string('a', 301), // limit is 300
            Duration = 100,
            ReleaseYear = 2024,
            Base64Image = string.Empty
        };

        var results = Validate(dto);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(MovieCreateDto.Title)));
    }

    [Fact]
    public void MovieCreateDto_DescriptionOverMaxLength_FailsValidation()
    {
        var dto = new MovieCreateDto
        {
            Title = "Valid Title",
            Description = new string('a', 4001), // limit is 4000
            Duration = 100,
            ReleaseYear = 2024,
            Base64Image = string.Empty
        };

        var results = Validate(dto);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(MovieCreateDto.Description)));
    }

    [Fact]
    public void MovieCreateDto_WithinAllLimits_PassesValidation()
    {
        var dto = new MovieCreateDto
        {
            Title = new string('a', 300),
            Description = new string('b', 4000),
            Duration = 120,
            ReleaseYear = 2024,
            Base64Image = string.Empty
        };

        var results = Validate(dto);

        Assert.Empty(results);
    }

    [Fact]
    public void UserRegisterDto_UsernameOverMaxLength_FailsValidation()
    {
        var dto = new UserRegisterDto
        {
            Username = new string('a', 101), // limit is 100
            Password = "password123"
        };

        var results = Validate(dto);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(UserRegisterDto.Username)));
    }

    [Fact]
    public void UserRegisterDto_UsernameUnderMinLength_FailsValidation()
    {
        var dto = new UserRegisterDto
        {
            Username = "ab", // minimum is 3
            Password = "password123"
        };

        var results = Validate(dto);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(UserRegisterDto.Username)));
    }

    [Fact]
    public void UserRegisterDto_EmailOverMaxLength_FailsValidation()
    {
        var localPart = new string('a', 315);
        var dto = new UserRegisterDto
        {
            Username = "validuser",
            Password = "password123",
            Email = $"{localPart}@example.com" // > 320 chars total, limit is 320
        };

        var results = Validate(dto);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(UserRegisterDto.Email)));
    }

    [Fact]
    public void UserRegisterDto_ValidInput_PassesValidation()
    {
        var dto = new UserRegisterDto
        {
            Username = "validuser",
            Password = "password123",
            Email = "user@example.com"
        };

        var results = Validate(dto);

        Assert.Empty(results);
    }

    [Fact]
    public void UserCreateDto_UsernameOverMaxLength_FailsValidation()
    {
        var dto = new UserCreateDto
        {
            Username = new string('a', 101),
            Password = "password123",
            Role = UserRole.User
        };

        var results = Validate(dto);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(UserCreateDto.Username)));
    }
}

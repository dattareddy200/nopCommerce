using AwesomeAssertions;
using Nop.Core;
using NUnit.Framework;

namespace Nop.Tests.Nop.Core.Tests;

[TestFixture]
public class CommonHelperEmailValidatorTests
{
    [Test]
    public void WhenTheTextIsAValidEmailAddressThenTheValidatorShouldPass()
    {
        var email = "testperson@gmail.com";
        var result = CommonHelper.IsValidEmail(email);
        result.Should().BeTrue();
    }

    [Test]
    public void WhenTheTextIsAValidEmailAddressIncludingPlusValidatorShouldPass()
    {
        var email = "testperson+label@gmail.com";
        var result = CommonHelper.IsValidEmail(email);
        result.Should().BeTrue();
    }

    [Test]
    public void WhenTheTextIsNullThenTheValidatorShouldFail()
    {
        var result = CommonHelper.IsValidEmail(null);
        result.Should().BeFalse();
    }

    [Test]
    public void WhenTheTextIsEmptyThenTheValidatorShouldFail()
    {
        var email = string.Empty;
        var result = CommonHelper.IsValidEmail(email);
        result.Should().BeFalse();
    }

    [Test]
    public void WhenTheTextIsNotAValidEmailAddressThenTheValidatorShouldFail()
    {
        const string email = "testperso";
        var result = CommonHelper.IsValidEmail(email);
        result.Should().BeFalse();
    }

    [Test]
    public void ThisShouldNotHang()
    {
        const string email = "thisisaverylongstringcodeplex.com";
        var result = CommonHelper.IsValidEmail(email);
        result.Should().BeFalse();
    }

    [Test]
    public void WhenEmailAddressContainsUpperCasesThenTheValidatorShouldPass()
    {
        var email = "testperson@gmail.com";
        var result = CommonHelper.IsValidEmail(email);
        result.Should().BeTrue();

        email = "TestPerson@gmail.com";
        result = CommonHelper.IsValidEmail(email);
        result.Should().BeTrue();
    }

    [Test]
    public void WhenEmailAddressContainsHyphenInDomainThenTheValidatorShouldPass()
    {
        //underscore is not allowed in domain part 
        var email = "testperson@domain_with-hyphen.com";
        var result = CommonHelper.IsValidEmail(email);
        result.Should().BeFalse();

        email = "testperson@example-domain.net";
        result = CommonHelper.IsValidEmail(email);
        result.Should().BeTrue();
    }

    [Test]
    public void WhenEmailAddressContainsDifferentTLDsThenTheValidatorShouldPass()
    {
        var email = "testperson@example.co.uk";
        var result = CommonHelper.IsValidEmail(email);
        result.Should().BeTrue();

        email = "testperson@example.museum";
        result = CommonHelper.IsValidEmail(email);
        result.Should().BeTrue();
    }

    [Test]
    public void ValidateEmailWithDetailsReturnsCorrectResultForValidEmail()
    {
        var email = "testperson@gmail.com";
        var (isValid, message) = CommonHelper.ValidateEmailWithDetails(email);
        isValid.Should().BeTrue();
        message.Should().BeEmpty();
    }

    [Test]
    public void ValidateEmailWithDetailsReturnsCorrectResultForInvalidEmail()
    {
        var email = "invalid-email";
        var (isValid, message) = CommonHelper.ValidateEmailWithDetails(email);
        isValid.Should().BeFalse();
        message.Should().NotBeEmpty();
    }

    [Test]
    public void ValidateEmailWithDetailsReturnsCorrectResultForEmptyEmail()
    {
        var (isValid, message) = CommonHelper.ValidateEmailWithDetails(string.Empty);
        isValid.Should().BeFalse();
        message.Should().Be("Email address is required");
    }

    [Test]
    public void ValidateEmailWithDetailsReturnsCorrectResultForWhitespaceOnlyEmail()
    {
        var (isValid, message) = CommonHelper.ValidateEmailWithDetails("   ");
        isValid.Should().BeFalse();
        message.Should().Be("Email address is required");
    }

    [Test]
    public void ValidateEmailWithDetailsReturnsCorrectResultForTooLongEmail()
    {
        var email = new string('a', 250) + "@test.com";
        var (isValid, message) = CommonHelper.ValidateEmailWithDetails(email);
        isValid.Should().BeFalse();
        message.Should().Be("Email address is too long (maximum 255 characters)");
    }

    [Test]
    public void IsDisposableEmailReturnsTrueForKnownDisposableDomains()
    {
        var email = "test@tempmail.com";
        var result = CommonHelper.IsDisposableEmail(email);
        result.Should().BeTrue();

        email = "user@mailinator.com";
        result = CommonHelper.IsDisposableEmail(email);
        result.Should().BeTrue();
    }

    [Test]
    public void IsDisposableEmailReturnsFalseForLegitimateEmails()
    {
        var email = "test@gmail.com";
        var result = CommonHelper.IsDisposableEmail(email);
        result.Should().BeFalse();

        email = "user@company.com";
        result = CommonHelper.IsDisposableEmail(email);
        result.Should().BeFalse();
    }

    [Test]
    public void ValidateEmailWithDetailsChecksDisposableWhenRequested()
    {
        var email = "test@tempmail.com";
        var (isValid, message) = CommonHelper.ValidateEmailWithDetails(email, checkDisposable: true);
        isValid.Should().BeFalse();
        message.Should().Be("Disposable email addresses are not allowed");
    }

    [Test]
    public void ValidateEmailWithDetailsDoesNotCheckDisposableByDefault()
    {
        var email = "test@tempmail.com";
        var (isValid, message) = CommonHelper.ValidateEmailWithDetails(email);
        isValid.Should().BeTrue();
        message.Should().BeEmpty();
    }
}
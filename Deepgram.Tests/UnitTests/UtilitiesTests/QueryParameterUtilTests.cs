﻿namespace Deepgram.Tests.UnitTests.UtilitiesTests;

public class QueryParameterUtilTests
{
    [Test]
    public void GetParameters_Should_Return_String_When_Passing_String_Parameter()
    {
        //Arrange
        var prerecordedOptions = new AutoFaker<PrerecordedSchema>().Generate();
        var expectedModel = HttpUtility.UrlEncode(prerecordedOptions.Model)!.ToLower();
        var expected = $"{nameof(prerecordedOptions.Model).ToLower()}={expectedModel}";
        //Act
        var SUT = QueryParameterUtil.GetParameters(prerecordedOptions);

        //Assert
        SUT.Should().NotBeNull();
        SUT.Should().Contain(expectedModel);
    }

    [Test]
    public void GetParameters_Should_Return_String_Respecting_Callback_Casing()
    {
        //Arrange
        var prerecordedOptions = new AutoFaker<PrerecordedSchema>().Generate();
        prerecordedOptions.Callback = "https://Signed23.com";
        var expected = $"{nameof(prerecordedOptions.Callback).ToLower()}={HttpUtility.UrlEncode("https://Signed23.com")}";
        //Act
        var SUT = QueryParameterUtil.GetParameters(prerecordedOptions);

        //Assert
        SUT.Should().NotBeNull();
        SUT.Should().Contain(expected);
    }

    [Test]
    public void GetParameters_Should_Return_String_When_Passing_Int_Parameter()
    {
        //Arrange 
        var obj = new PrerecordedSchema() { Alternatives = 1 };
        var expected = $"alternatives={obj.Alternatives}";

        //Act
        var SUT = QueryParameterUtil.GetParameters(obj);

        //Assert
        SUT.Should().NotBeNull();
        SUT.Should().Contain(expected);
    }

    [Test]
    public void GetParameters_Should_Return_String_When_Passing_Array_Parameter()
    {
        //Arrange
        var prerecordedOptions = new PrerecordedSchema
        {
            Keywords = new string[] { "test" }
        };
        var expected = $"keywords={prerecordedOptions.Keywords[0].ToLower()}";

        //Act
        var SUT = QueryParameterUtil.GetParameters(prerecordedOptions);

        //Assert
        SUT.Should().NotBeNull();
        SUT.Should().Contain(expected);
    }

    [Test]
    public void GetParameters_Should_Return_String_When_Passing_Decimal_Parameter()
    {
        //Arrange
        var prerecordedOptions = new PrerecordedSchema() { UtteranceSplit = 2.3 };
        var expected = $"utt_split={HttpUtility.UrlEncode(prerecordedOptions.UtteranceSplit.ToString())}";

        //Act
        // need to set manual as the precision can be very long and gets trimmed from autogenerated value

        var SUT = QueryParameterUtil.GetParameters(prerecordedOptions);

        //Assert
        SUT.Should().NotBeNull();
        SUT.Should().Contain(expected);
    }

    [Test]
    public void GetParameters_Should_Return_String_When_Passing_Boolean_Parameter()
    {
        //Arrange 
        var obj = new PrerecordedSchema() { Paragraphs = true };
        var expected = $"{nameof(obj.Paragraphs).ToLower()}=true";
        //Act
        var SUT = QueryParameterUtil.GetParameters(obj);

        //Assert
        SUT.Should().NotBeNull();
        SUT.Should().Contain(expected);
    }

    [Test]
    public void GetParameters_Should_Return_String_When_Passing_DateTime_Parameter()
    {
        //Arrange 
        var obj = DateTime.Now;
        var option = new CreateProjectKeyWithExpirationSchema()
        {
            ExpirationDate = obj
        };
        var expected = $"expiration_date={HttpUtility.UrlEncode(obj.ToString("yyyy-MM-dd"))}";

        //Act
        var SUT = QueryParameterUtil.GetParameters(option);

        //Assert
        SUT.Should().NotBeNull();
        SUT.Should().Contain(expected); ;
    }


    [Test]
    public void GetParameters_Should_Return_Empty_String_When_Parameter_Has_No_Values()
    {
        //Act
        var SUT = QueryParameterUtil.GetParameters(new PrerecordedSchema());

        //Assert
        SUT.Should().NotBeNull();
        SUT.Should().Be(string.Empty);
    }
}
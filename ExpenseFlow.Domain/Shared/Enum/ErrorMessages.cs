namespace ExpenseFlow.Domain.Shared.Enum
{
    public class ErrorMessages
    {

        public static string NotFound => nameof(NotFound);
        public static string TagNotFound => nameof(TagNotFound);
        public static string FileIsEmpty => nameof(FileIsEmpty);
        public static string RoleHasUsers => nameof(RoleHasUsers);
        public static string UserNotFound => nameof(UserNotFound);
        public static string RoleNotFound => nameof(RoleNotFound);
        public static string CityNotFound => nameof(CityNotFound);
        public static string TaskNotFound => nameof(TaskNotFound);
        public static string UnAuthorized => nameof(UnAuthorized);
        public static string PostNotFound => nameof(PostNotFound);
        public static string EmailNotValid => nameof(EmailNotValid);
        public static string ClientNotFound => nameof(ClientNotFound);
        public static string VisitNotFound => nameof(VisitNotFound);
        public static string MissingColumns => nameof(MissingColumns);
        public static string CourseNotFound => nameof(CourseNotFound);
        public static string UserIsRequired => nameof(UserIsRequired);
        public static string TaskIsRequired => nameof(TaskIsRequired);
        public static string UnAuthenticated => nameof(UnAuthenticated);
        public static string TrainerNotFound => nameof(TrainerNotFound);
        public static string CountryNotFound => nameof(CountryNotFound);
        public static string visitorNotFound => nameof(visitorNotFound);
        public static string AudienceNotFound => nameof(AudienceNotFound);
        public static string CategoryNotFound => nameof(CategoryNotFound);
        public static string IndustryNotFound => nameof(IndustryNotFound);
        public static string InvalidDateRange => nameof(InvalidDateRange);
        public static string AudienceAlreadyExists => nameof(AudienceAlreadyExists);
        public static string InvalidMediaType => nameof(InvalidMediaType);
        public static string CourseIsRequired => nameof(CourseIsRequired);
        public static string EmailAlreadyExist => nameof(EmailAlreadyExist);
        public static string InvalidFileFormat => nameof(InvalidFileFormat);
        public static string NameCanNotBeEmpty => nameof(NameCanNotBeEmpty);
        public static string VideoAlreadyExists => nameof(VideoAlreadyExists);
        public static string CategoryHasCourses => nameof(CategoryHasCourses);
        public static string CourseSeatsAreFull => nameof(CourseSeatsAreFull);
        public static string IndustryIsRequired => nameof(IndustryIsRequired);
        public static string PasswordNotCorrect => nameof(PasswordNotCorrect);
        public static string PasswordPolicyError => nameof(PasswordPolicyError);
        public static string JoinRequestNotFound => nameof(JoinRequestNotFound);
        public static string IsValidFileExtension => nameof(IsValidFileExtension);
        public static string JoinRequestIsRequired => nameof(JoinRequestIsRequired);
        public static string ConsultationNotFound => nameof(ConsultationNotFound);
        public static string CompanyNameIsRequired => nameof(CompanyNameIsRequired);
        public static string InvalidCourseStatus => nameof(InvalidCourseStatus);
        public static string SettingKeyNotFound => nameof(SettingKeyNotFound);

        public static string OldPasswordCanNotBeEmpty => nameof(OldPasswordCanNotBeEmpty);
        public static string NewPasswordCanNotBeEmpty => nameof(NewPasswordCanNotBeEmpty);
        public static string PermissionsCanNotBeEmpty => nameof(PermissionsCanNotBeEmpty);
        public static string CannotDeleteUserHasTasks => nameof(CannotDeleteUserHasTasks);

        public static string LeadStatusAlreadySet => nameof(LeadStatusAlreadySet);
        public static string LeadAlreadyContact => nameof(LeadAlreadyContact);
        public static string DealNotFound => nameof(DealNotFound);
        public static string DealSourceRequired => nameof(DealSourceRequired);
        public static string DealCodeAlreadyExists => nameof(DealCodeAlreadyExists);
        public static string InvalidDiscount => nameof(InvalidDiscount);
        public static string FileExtensionNotValid => nameof(FileExtensionNotValid);
        public static string EmailAlreadyConfirmed => nameof(EmailAlreadyConfirmed);
        public static string ConsultationIsRequired => nameof(ConsultationIsRequired);
        public static string InvalidEmailOrPassword => nameof(InvalidEmailOrPassword);
        public static string InvalidCategoryForPost => nameof(InvalidCategoryForPost);
        public static string CourseEnrollmentClosed => nameof(CourseEnrollmentClosed);
        public static string InvalidConfirmationCode => nameof(InvalidConfirmationCode);
        public static string CourseRegistrationClosed => nameof(CourseRegistrationClosed);
        public static string InvalidCategoryForCourse => nameof(InvalidCategoryForCourse);
        public static string CannotChangeCategoryType => nameof(CannotChangeCategoryType);
        public static string ImageSizeMustBeLessThan5MB => nameof(ImageSizeMustBeLessThan5MB);
        public static string OneOrMorePermissionNotFound => nameof(OneOrMorePermissionNotFound);
        public static string ImageSizeMustBeLessThan1024 => nameof(ImageSizeMustBeLessThan1024);
        public static string Phoneoremailisrequired => nameof(Phoneoremailisrequired);


        public static string DescribeYourNeedsIsRequired => nameof(DescribeYourNeedsIsRequired);
        public static string VideoSizeMustBeLessThan50MB => nameof(VideoSizeMustBeLessThan50MB);
        public static string YouAlreadyAppliedToThisCourse => nameof(YouAlreadyAppliedToThisCourse);
        public static string YouCanOnlyAddMediaTopastCourse => nameof(YouCanOnlyAddMediaTopastCourse);
        public static string CanNotDeleteRoleAssignedToUser => nameof(CanNotDeleteRoleAssignedToUser);
        public static string InvalidCategoryForConsultation => nameof(InvalidCategoryForConsultation);
        public static string YouCantDeleteCourseWithStudents => nameof(YouCantDeleteCourseWithStudents);
        public static string YouCanOnlyApplyToUpcomingCourse => nameof(YouCanOnlyApplyToUpcomingCourse);
        public static string SomePermissionsNotFoundOrInvalid => nameof(SomePermissionsNotFoundOrInvalid);
        public static string CannotDeleteUserHasAssignedLeads => nameof(CannotDeleteUserHasAssignedLeads);
        public static string YouCantDeleteCourseWithOpenEnrollment => nameof(YouCantDeleteCourseWithOpenEnrollment);
        public static string CanNotDeleteCityBecauseitdisplayscourse => nameof(CanNotDeleteCityBecauseitdisplayscourse);
        public static string CanNotDeleteCountryBecauseitdisplayscity => nameof(CanNotDeleteCountryBecauseitdisplayscity);
        public static string YouCanOnlySetComingSoonForUpcomingCourse => nameof(YouCanOnlySetComingSoonForUpcomingCourse);
        public static string PleaseChooseAnExistingIndustryOrAddNewOne => nameof(PleaseChooseAnExistingIndustryOrAddNewOne);
        public static string YouCanOnlyOpenEnrollmentForUpcomingCourse => nameof(YouCanOnlyOpenEnrollmentForUpcomingCourse);
        public static string YouCannotDeleteTheConsultationItHasARequest => nameof(YouCannotDeleteTheConsultationItHasARequest);
        public static string YouCantDeleteTrainerBecauseAssignedToCourse => nameof(YouCantDeleteTrainerBecauseAssignedToCourse);
        public static string CannotOpenEnrollmentAfterRegistrationCloseDate => nameof(CannotOpenEnrollmentAfterRegistrationCloseDate);


    }

}

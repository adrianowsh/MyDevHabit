namespace MyDevHabit.Api.Services;

public static class CustomMediaTypeNames
{
    internal static class Application
    {
        public const string JsonV1 = "application/json;v=1";
        public const string JsonV2 = "application/json;v=2";
        public const string HateoasJson = " ";
        public const string HateoasJsonV1 = "application/vnd.mydevhabit.hateoas.1+json";
        public const string HateoasJsonV2 = "application/vnd.mydevhabit.hateoas.2+json";

        public const string HateoasSubType = "hateoas";
    }
}

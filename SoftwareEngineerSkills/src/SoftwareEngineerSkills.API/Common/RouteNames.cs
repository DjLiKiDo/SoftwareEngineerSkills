namespace SoftwareEngineerSkills.API.Common;

/// <summary>
/// Constants for API route names used with CreatedAtRoute and routing attributes
/// </summary>
public static class RouteNames
{
    /// <summary>
    /// Route names for Dummy controller
    /// </summary>
    public static class Dummy
    {
        public const string GetById = nameof(GetById);
        public const string GetAll = nameof(GetAll);
        public const string Create = nameof(Create);
        public const string Update = nameof(Update);
        public const string Delete = nameof(Delete);
        public const string Activate = nameof(Activate);
        public const string Deactivate = nameof(Deactivate);
    }
}

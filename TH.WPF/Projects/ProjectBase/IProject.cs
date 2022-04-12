namespace ProjectBase
{
    public interface IProject
    {
        /// <summary>
        /// Title of the project
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Conditions to determine if the project can open selected file or folder
        /// </summary>
        /// <returns></returns>
        bool IsValid();

        /// <summary>
        /// Open actions for the project
        /// </summary>
        /// <returns></returns>
        bool Open();

        /// <summary>
        /// Save actions for the project
        /// </summary>
        /// <returns></returns>
        bool Save();
    }
}
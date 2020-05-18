namespace middler.Variables
{
    public static class IVariableInfoExtensions
    {
        public static Variable ToVariable(this IVariableInfo variableInfo, object content = null)
        {
            var variable = new Variable();
            variable.Name = variableInfo.Name;
            variable.Extension = variableInfo.Extension;
            variable.Parent = variableInfo.Parent;
            variable.Flags = variableInfo.Flags;
            variable.CreatedAt = variableInfo.CreatedAt;
            variable.UpdatedAt = variableInfo.UpdatedAt;
            variable.FullPath = variableInfo.FullPath;
            variable.IsFolder = variableInfo.IsFolder;
            variable.Content = content;
            return variable;
        }

    }
}
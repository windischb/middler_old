namespace middler.Variables
{
    public interface IVariable: IVariableInfo
    {
        object Content { get; set; }
    }
}
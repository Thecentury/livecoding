namespace LiveCoding.Core
{
	public interface IMemberValue
	{
		string MemberName { get; }
		
		object GetValue();
	}
}
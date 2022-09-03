namespace ViewModels.Pages.Admin.PostCategories;

public class IndexItemViewModel : object
{
	public IndexItemViewModel() : base()
	{
		Title = string.Empty;
	}

	// **********
	public System.Guid Id { get; set; }
	// **********

	// **********
	public bool IsActive { get; set; }
	// **********

	// **********
	public bool IsUndeletable { get; set; }
	// **********

	// **********
	public string Title { get; set; }
	// **********

	// **********
	public int Ordering { get; set; }
	// **********

	// **********
	public int SubPostCategoriesCount { get; set; }
	// **********

	// **********
	public System.DateTime InsertDateTime { get; set; }
	// **********

	// **********
	public System.DateTime UpdateDateTime { get; set; }
	// **********
}

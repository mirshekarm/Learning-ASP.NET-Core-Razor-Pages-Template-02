namespace ViewModels.Pages.Admin.PostCategories;

public class DetailsOrDeleteViewModel : UpdateViewModel
{
	public DetailsOrDeleteViewModel() : base()
	{
		InsertDateTime =
			Domain.SeedWork.Utility.Now;

		UpdateDateTime =
			Domain.SeedWork.Utility.Now;
	}

	// **********
	[System.ComponentModel.DataAnnotations.Display
		(ResourceType = typeof(Resources.DataDictionary),
		Name = nameof(Resources.DataDictionary.SubPostCategoriesCount))]
	public int SubPostCategoryCount { get; set; }
	// **********

	// **********
	// **********
	// **********
	public string? ParentTitle { get; set; }
	// **********

	// **********
	[System.ComponentModel.DataAnnotations.Display
	(ResourceType = typeof(Resources.DataDictionary),
	Name = nameof(Resources.DataDictionary.Parent))]
	public string DisplayParentTitle
	{
		get
		{
			if (string.IsNullOrWhiteSpace(ParentTitle))
			{
				var noParentTitle =
					Resources.DataDictionary.NoParent;

				return noParentTitle;
			}

			return ParentTitle;
		}
	}
	// **********
	// **********
	// **********

	// **********
	// **********
	// **********
	public System.DateTime InsertDateTime { get; set; }
	// **********

	// **********
	[System.ComponentModel.DataAnnotations.Display
		(ResourceType = typeof(Resources.DataDictionary),
		Name = nameof(Resources.DataDictionary.InsertDateTime))]
	public string DisplayInsertDateTime
	{
		get
		{
			var result =
				InsertDateTime.ToString
				(format: Constants.Format.DateTime);

			return result;
		}
	}
	// **********
	// **********
	// **********

	// **********
	// **********
	// **********
	public System.DateTime UpdateDateTime { get; set; }
	// **********
	// **********

	// **********
	[System.ComponentModel.DataAnnotations.Display
		(ResourceType = typeof(Resources.DataDictionary),
		Name = nameof(Resources.DataDictionary.UpdateDateTime))]
	public string DisplayUpdateDateTime
	{
		get
		{
			var result =
				UpdateDateTime.ToString
				(format: Constants.Format.DateTime);

			return result;
		}
	}
	// **********
	// **********
	// **********
}

using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Server.Pages.Admin.PostCategories;

[Microsoft.AspNetCore.Authorization
	.Authorize(Roles = Constants.Role.Admin)]
public class UpdateModel : Infrastructure.BasePageModelWithDatabaseContext
{
	#region Constractor
	public UpdateModel
		(Data.DatabaseContext databaseContext,
		Microsoft.Extensions.Logging.ILogger<UpdateModel> logger) :
		base(databaseContext: databaseContext)
	{
		Logger = logger;

		ViewModel = new();

		ParentsViewModel = new System.Collections.Generic.List
			<ViewModels.Pages.Admin.PostCategory.ParentsViewModel>();
	}
	#endregion /Constractor

	#region Public Property(ies)
	// **********
	private Microsoft.Extensions.Logging.ILogger<UpdateModel> Logger { get; }
	// **********

	// **********
	[Microsoft.AspNetCore.Mvc.BindProperty]
	public ViewModels.Pages.Admin.PostCategories.UpdateViewModel ViewModel { get; set; }
	// **********

	// **********
	public System.Collections.Generic.IList
		<ViewModels.Pages.Admin.PostCategory.ParentsViewModel> ParentsViewModel
	{ get; private set; }
	// **********
	#endregion /Public Property(ies)

	#region OnGetAsync
	public async System.Threading.Tasks.Task
	<Microsoft.AspNetCore.Mvc.IActionResult> OnGetAsync(System.Guid? id)
	{
		try
		{
			if (id.HasValue == false)
			{
				AddToastError
					(message: Resources.Messages.Errors.IdIsNull);

				return RedirectToPage(pageName: "Index");
			}

			ViewModel =
				await
				DatabaseContext.PostCategories
				.Where(current => current.Id == id.Value)
				.Select(current => new ViewModels.Pages.Admin.PostCategories.UpdateViewModel()
				{
					Id = current.Id,
					Title = current.Title,
					IsActive = current.IsActive,
					Ordering = current.Ordering,
					Description = current.Description,
					ParentId = current.ParentId,
				})
				.FirstOrDefaultAsync();

			if (ViewModel == null)
			{
				AddToastError
					(message: Resources.Messages.Errors.ThereIsNotAnyDataWithThisId);

				return RedirectToPage(pageName: "Index");
			}

			await SetAccessibleParent(id: id);

			return Page();
		}
		catch (System.Exception ex)
		{
			Logger.LogError
				(message: Constants.Logger.ErrorMessage, args: ex.Message);

			AddToastError
				(message: Resources.Messages.Errors.UnexpectedError);

			return RedirectToPage(pageName: "Index");
		}
		finally
		{
			await DisposeDatabaseContextAsync();
		}
	}
	#endregion /OnGetAsync

	#region OnPostAsync
	public async System.Threading.Tasks.Task
	<Microsoft.AspNetCore.Mvc.IActionResult> OnPostAsync()
	{
		if (ModelState.IsValid == false)
		{
			return Page();
		}

		try
		{
			// **************************************************
			var foundedItem =
				await
				DatabaseContext.PostCategories
				.Where(current => current.Id == ViewModel.Id)
				.FirstOrDefaultAsync();

			if (foundedItem == null)
			{
				AddToastError
					(message: Resources.Messages.Errors.ThereIsNotAnyDataWithThisId);

				return RedirectToPage(pageName: "Index");
			}
			// **************************************************

			// **************************************************
			var fixedTitle =
				Dtat.Utility.FixText
				(text: ViewModel.Title);

			var foundedAny =
				await
				DatabaseContext.PostCategories
				.Where(current => current.Id != ViewModel.Id)
				.Where(current => current.Title.ToLower() == fixedTitle.ToLower())
				.AnyAsync();

			if (foundedAny)
			{
				// **************************************************
				var errorMessage = string.Format
					(Resources.Messages.Errors.AlreadyExists,
					Resources.DataDictionary.PostCategory);

				AddPageError(message: errorMessage);
				// **************************************************

				return Page();
			}
			// **************************************************

			// **************************************************
			if (ViewModel.ParentId != null && ViewModel.ParentId != foundedItem.ParentId)
			{
				var parentSelectedTitle =
					await DatabaseContext.PostCategories
					.Where(current => current.Id == ViewModel.ParentId)
					.Select(current => current.Title)
					.FirstOrDefaultAsync();

				if (string.IsNullOrWhiteSpace(parentSelectedTitle))
				{
					var errorMessage =
						string.Format(Resources.Messages.Errors.NotFound, Resources.DataDictionary.Parent);

					AddPageError(message: errorMessage);

					return Page();
				}

				if (parentSelectedTitle == fixedTitle)
				{
					var errorMessage = string.Format
						(Resources.Messages.Errors.AlreadyExists,
						Resources.DataDictionary.PostCategory);

					AddPageError(message: errorMessage);

					return Page();
				}
			}
			// **************************************************

			// **************************************************
			var fixedDescription =
				Dtat.Utility.FixText
				(text: ViewModel.Description);

			foundedItem.SetUpdateDateTime();

			foundedItem.Title = fixedTitle;
			foundedItem.Ordering = ViewModel.Ordering;
			foundedItem.IsActive = ViewModel.IsActive;
			foundedItem.Description = fixedDescription;
			foundedItem.ParentId = ViewModel.ParentId;
			// **************************************************

			var affectedRows =
				await
				DatabaseContext.SaveChangesAsync();

			// **************************************************
			var successMessage = string.Format
				(Resources.Messages.Successes.Updated,
				Resources.DataDictionary.PageCategory);

			AddToastSuccess(message: successMessage);
			// **************************************************

			return RedirectToPage(pageName: "Index");
		}
		catch (System.Exception ex)
		{
			Logger.LogError
				(message: Constants.Logger.ErrorMessage, args: ex.Message);

			AddToastError
				(message: Resources.Messages.Errors.UnexpectedError);

			return RedirectToPage(pageName: "Index");
		}
		finally
		{
			await DisposeDatabaseContextAsync();
		}
	}
	#endregion /OnPostAsync

	#region SetAccessibleParent
	public async System.Threading.Tasks.Task SetAccessibleParent(System.Guid? id)
	{
		var parentsCategories =
			await DatabaseContext.PostCategories
			.Where(current => current.ParentId == null)
			.Where(current => current.Id != id)
			.ToListAsync();

		var selectViewModel =
			new System.Collections.Generic.List
				<ViewModels.Pages.Admin.PostCategory.ParentsViewModel>();

		//First Level : Parentns
		foreach (var parent in parentsCategories)
		{
			if (parent.Id != id)
			{
				selectViewModel.Add(new ViewModels.Pages.Admin.PostCategory.ParentsViewModel
				{
					Id = parent.Id,
					Title = parent.Title,
				});
			}

			var firstLevelChildren = parent.SubCategories;

			//Secound Level : Parentns > First Children
			foreach (var child in firstLevelChildren)
			{
				var treeTitle = new System.Text.StringBuilder();

				treeTitle.Append(parent.Title);

				treeTitle.Append(" > ");

				treeTitle.Append(child.Title);

				if (child.Id != id)
				{
					selectViewModel.Add(new ViewModels.Pages.Admin.PostCategory.ParentsViewModel
					{
						Id = child.Id,
						Title = treeTitle.ToString(),
					});
				}

				var currentChildren = child.SubCategories;

				//Third Level : Parentns > First Children > ...
				while (currentChildren.Count > 0)
				{
					foreach (var currentChild in currentChildren)
					{
						treeTitle.Append(" > ");

						treeTitle.Append(currentChild.Title);

						if (currentChild.Id != id)
						{
							selectViewModel.Add(new ViewModels.Pages.Admin.PostCategory.ParentsViewModel
							{
								Id = currentChild.Id,
								Title = treeTitle.ToString(),
							});
						}

						currentChildren = currentChild.SubCategories;
					}
				}
			}
		}
		selectViewModel =
			selectViewModel.OrderBy(current => current.Title).ToList();

		ParentsViewModel = selectViewModel;
	}
	#endregion /SetAccessibleParent
}

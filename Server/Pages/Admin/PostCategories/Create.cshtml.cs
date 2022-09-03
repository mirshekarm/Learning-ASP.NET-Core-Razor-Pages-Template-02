using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Server.Pages.Admin.PostCategories;

[Microsoft.AspNetCore.Authorization.Authorize
	(Roles = Constants.Role.Admin)]
public class CreateModel : Infrastructure.BasePageModelWithDatabaseContext
{
	#region Constractor
	public CreateModel
		(Data.DatabaseContext databaseContext,
		Microsoft.Extensions.Logging.ILogger<CreateModel> logger) : base(databaseContext)
	{
		Logger = logger;

		ViewModel = new();

		ParentsViewModel = new System.Collections.Generic.List
			<ViewModels.Pages.Admin.PostCategory.ParentsViewModel>();
	}
	#endregion /Constractor

	#region Public Property(ies)
	// **********
	public Microsoft.Extensions.Logging.ILogger<CreateModel> Logger { get; }
	// **********

	// **********
	[Microsoft.AspNetCore.Mvc.BindProperty]
	public ViewModels.Pages.Admin.PostCategories.CreateViewModel ViewModel { get; set; }
	// **********

	// **********
	public System.Collections.Generic.IList
		<ViewModels.Pages.Admin.PostCategory.ParentsViewModel> ParentsViewModel
	{ get; private set; }
	// **********
	#endregion /Public Property(ies)

	#region OnGetAsync
	public async System.Threading.Tasks.Task
		<Microsoft.AspNetCore.Mvc.IActionResult> OnGetAsync()
	{
		try
		{
			await SetAccessibleParent();

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
			return RedirectToPage(pageName: "Create");
		}

		try
		{
			var fixedTitle =
				Dtat.Utility.FixText
				(text: ViewModel.Title);

			var foundedAny =
				await DatabaseContext.PostCategories
				.Where(current => current.ParentId == ViewModel.ParentId)
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
			if (ViewModel.ParentId != null)
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

			var newEntity =
				new Domain.PostCategory(title: fixedTitle)
				{
					Description = fixedDescription,
					ParentId = ViewModel.ParentId,
					IsActive = ViewModel.IsActive,
					Ordering = ViewModel.Ordering,
				};

			var entityEntry =
				await
				DatabaseContext.AddAsync(entity: newEntity);

			var affectedRows =
				await
				DatabaseContext.SaveChangesAsync();
			// **************************************************

			// **************************************************
			var successMessage = string.Format
				(Resources.Messages.Successes.Created,
				Resources.DataDictionary.PostCategory);

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
	public async System.Threading.Tasks.Task SetAccessibleParent()
	{
		var parentsCategories =
			await DatabaseContext.PostCategories
			.Where(current => current.ParentId == null)
			.ToListAsync();

		var selectViewModel =
			new System.Collections.Generic.List
				<ViewModels.Pages.Admin.PostCategory.ParentsViewModel>();

		//First Level : Parentns
		foreach (var parent in parentsCategories)
		{
			selectViewModel.Add(new ViewModels.Pages.Admin.PostCategory.ParentsViewModel
			{
				Id = parent.Id,
				Title = parent.Title,
			});

			var firstLevelChildren = parent.SubCategories;

			//Secound Level : Parentns > First Children
			foreach (var child in firstLevelChildren)
			{
				var treeTitle = new System.Text.StringBuilder();

				treeTitle.Append(parent.Title);

				treeTitle.Append(" > ");

				treeTitle.Append(child.Title);

				selectViewModel.Add(new ViewModels.Pages.Admin.PostCategory.ParentsViewModel
				{
					Id = child.Id,
					Title = treeTitle.ToString(),
				});

				var currentChildren = child.SubCategories;

				//Third Level : Parentns > First Children > ...
				while (currentChildren.Count > 0)
				{
					foreach (var currentChild in currentChildren)
					{
						treeTitle.Append(" > ");

						treeTitle.Append(currentChild.Title);

						selectViewModel.Add(new ViewModels.Pages.Admin.PostCategory.ParentsViewModel
						{
							Id = currentChild.Id,
							Title = treeTitle.ToString(),
						});

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

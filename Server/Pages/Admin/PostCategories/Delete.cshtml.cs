using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Server.Pages.Admin.PostCategories;

[Microsoft.AspNetCore.Authorization.Authorize
	(Roles = Constants.Role.Admin)]
public class DeleteModel : Infrastructure.BasePageModelWithDatabaseContext
{
	#region Constractor
	public DeleteModel
		(Data.DatabaseContext databaseContext,
		Microsoft.Extensions.Logging.ILogger<DeleteModel> logger) :
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
	private Microsoft.Extensions.Logging.ILogger<DeleteModel> Logger { get; }
	// **********

	// **********
	[Microsoft.AspNetCore.Mvc.BindProperty]
	public ViewModels.Pages.Admin.PostCategories.DetailsOrDeleteViewModel ViewModel { get; private set; }
	// **********

	// **********
	public System.Collections.Generic.IList
		<ViewModels.Pages.Admin.PostCategory.ParentsViewModel> ParentsViewModel{ get; private set; }
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
				.Select(current => new ViewModels.Pages.Admin.PostCategories.DetailsOrDeleteViewModel()
				{
					Id = current.Id,
					Title = current.Title,
					IsActive = current.IsActive,
					Ordering = current.Ordering,
					SubPostCategoryCount = current.SubCategories.Count,
					Description = current.Description,
					InsertDateTime = current.InsertDateTime,
					UpdateDateTime = current.UpdateDateTime,
					ParentTitle = current.Parent.Title
				})
				.FirstOrDefaultAsync();

			if (ViewModel == null)
			{
				AddToastError
					(message: Resources.Messages.Errors.ThereIsNotAnyDataWithThisId);

				return RedirectToPage(pageName: "Index");
			}

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
	<Microsoft.AspNetCore.Mvc.IActionResult> OnPostAsync(System.Guid? id)
	{
		try
		{
			// **************************************************
			if (id.HasValue == false)
			{
				AddToastError
					(message: Resources.Messages.Errors.IdIsNull);

				return RedirectToPage(pageName: "Index");
			}
			// **************************************************

			// **************************************************
			var hasAnyChildren =
				await
				DatabaseContext.PostCategories
				.Where(current => current.Id == id)
				.Where(current => current.SubCategories.Count > 0)
				.AnyAsync();

			if (hasAnyChildren)
			{
				// **************************************************
				var errorMessage = string.Format
					(Resources.Messages.Errors.CascadeDelete,
					Resources.DataDictionary.PostCategory);

				AddToastError(message: errorMessage);
				// **************************************************

				return RedirectToPage(pageName: "Index");
			}
			// **************************************************

			// **************************************************
			var foundedItem =
				await
				DatabaseContext.PostCategories
				.Where(current => current.Id == id.Value)
				.FirstOrDefaultAsync();

			if (foundedItem == null)
			{
				AddToastError
					(message: Resources.Messages.Errors.ThereIsNotAnyDataWithThisId);

				return RedirectToPage(pageName: "Index");
			}
			// **************************************************

			// **************************************************
			var entityEntry =
				DatabaseContext.Remove(entity: foundedItem);

			var affectedRows =
				await
				DatabaseContext.SaveChangesAsync();
			// **************************************************

			// **************************************************
			var successMessage = string.Format
				(Resources.Messages.Successes.Deleted,
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
}

using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Server.Pages.Admin.PostCategories;

[Microsoft.AspNetCore.Authorization
	.Authorize(Roles = Constants.Role.Admin)]
public class DetailsModel : Infrastructure.BasePageModelWithDatabaseContext
{
	#region Constractor
	public DetailsModel
		(Data.DatabaseContext databaseContext,
		Microsoft.Extensions.Logging.ILogger<DetailsModel> logger) :
		base(databaseContext: databaseContext)
	{
		Logger = logger;

		ViewModel = new();
	}
	#endregion /Constractor

	#region Public Property(ies)
	// **********
	private Microsoft.Extensions.Logging.ILogger<DetailsModel> Logger { get; }
	// **********

	// **********
	public ViewModels.Pages.Admin.PostCategories.DetailsOrDeleteViewModel ViewModel { get; private set; }
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
					ParentId = current.ParentId,
					Ordering = current.Ordering,
					Description = current.Description,
					ParentTitle = current.Parent.Title,
					InsertDateTime = current.InsertDateTime,
					UpdateDateTime = current.UpdateDateTime,
					SubPostCategoryCount = current.SubCategories.Count,
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
}

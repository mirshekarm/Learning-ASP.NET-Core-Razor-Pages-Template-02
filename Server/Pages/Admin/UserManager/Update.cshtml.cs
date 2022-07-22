using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Server.Pages.Admin.UserManager
{
	//[Microsoft.AspNetCore.Authorization.Authorize
	//	(Roles = Domain.SeedWork.Constant.SystemicRole.Admin)]
	public class UpdateModel : Infrastructure.BasePageModelWithDatabase
	{
		#region Constructor(s)
		public UpdateModel
			(Persistence.DatabaseContext databaseContext,
			Microsoft.Extensions.Logging.ILogger<UpdateModel> logger) : base(databaseContext: databaseContext)
		{
			Logger = logger;

			ViewModel = new();

			RolesViewModel = new System.Collections.Generic.List
				<ViewModels.Pages.Admin.UserManager.GetAccessibleRolesViewModel>();
		}
		#endregion /Constructor(s)

		#region Property(ies)
		// **********
		private Microsoft.Extensions.Logging.ILogger<UpdateModel> Logger { get; }
		// **********

		// **********
		public System.Collections.Generic.IList
			<ViewModels.Pages.Admin.UserManager.GetAccessibleRolesViewModel> RolesViewModel
		{ get; private set; }
		// **********
		#endregion /Property(ies)

		#region BindProperty(ies)
		// **********
		[Microsoft.AspNetCore.Mvc.BindProperty]
		public ViewModels.Pages.Admin.UserManager.UpdateUserViewModel ViewModel { get; set; }
		// **********
		#endregion /BindProperty(ies)

		#region OnGet
		public async System.Threading.Tasks.Task
			<Microsoft.AspNetCore.Mvc.IActionResult> OnGetAsync(System.Guid id)
		{
			try
			{
				ViewModel = await DatabaseContext.Users
					.Where(current => current.Id == id)
					.Select(current => new ViewModels.Pages.Admin.UserManager.UpdateUserViewModel
					{
						Id = current.Id,
						RoleId = current.RoleId,
						Username = current.Username,
						Ordering = current.Ordering,
						IsActive = current.IsActive,
						IsUndeletable = current.IsUndeletable,
					}).FirstOrDefaultAsync();
			}
			catch (System.Exception ex)
			{
				Logger.LogError(message: ex.Message);

				AddPageError(message: Resources.Messages.Errors.UnexpectedError);
			}
			finally
			{
				await SetAccessibleRole();

				await DisposeDatabaseContextAsync();
			}

			return Page();
		}
		#endregion /OnGet

		#region OnPost
		public async System.Threading.Tasks.Task
			<Microsoft.AspNetCore.Mvc.IActionResult> OnPostAsync(System.Guid id)
		{
			if (ModelState.IsValid is false)
			{
				return Page();
			}

			try
			{
				var foundedItem =
					await DatabaseContext.Users
					.Where(current => current.Id == id)
					.FirstOrDefaultAsync();

				if (foundedItem == null)
				{
					string errorMessage = string.Format
						(Resources.Messages.Errors.NotFound,
						Resources.DataDictionary.User);

					AddToastError(message: errorMessage);

					return RedirectToPage("./Index");
				}
				else
				{
					foundedItem.SetUpdateDateTime();
					foundedItem.Ordering = ViewModel.Ordering;
					foundedItem.IsActive = ViewModel.IsActive;
					foundedItem.RoleId = ViewModel.RoleId.Value;
					foundedItem.IsUndeletable = ViewModel.IsUndeletable;

					// **************************************************
					var isValid =
						Domain.SeedWork.ValidationHelper.IsValid(entity: foundedItem);

					var results =
						Domain.SeedWork.ValidationHelper.GetValidationResults(entity: foundedItem);
					// **************************************************

					if (isValid)
					{
						int affectedRows =
							await DatabaseContext.SaveChangesAsync();

						string successMessage = string.Format
							(Resources.Messages.Successes.SuccessfullyUpdated,
							Resources.DataDictionary.User);

						AddToastSuccess(message: successMessage);
					}

					return RedirectToPage("./Index");
				}
			}
			catch (System.Exception ex)
			{
				Logger.LogError(message: ex.Message);

				//System.Console.WriteLine(value: ex.Message);

				AddToastError(message: Resources.Messages.Errors.UnexpectedError);

				return Page();
			}
			finally
			{
				await SetAccessibleRole();

				await DisposeDatabaseContextAsync();
			}
		}
		#endregion /OnPost

		#region SetAccessibleRole
		private async System.Threading.Tasks.Task SetAccessibleRole()
		{
			RolesViewModel =
				await DatabaseContext.Roles
				//.Where(current => current.IsDeleted == false)
				.OrderBy(current => current.Ordering)
				.Select(current => new ViewModels.Pages.Admin.UserManager.GetAccessibleRolesViewModel
				{
					Id = current.Id,
					Name = current.Name,
				})
				.ToListAsync()
				;
		}
		#endregion /SetAccessibleRole
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.Toolkit.Events;

namespace TaxiOnline.Logic.Helpers
{
    internal static class UpdateHelper
    {
        public static ActionResult<IEnumerable<TModel>> EnumerateModels<TModel, TLogic>(Func<ActionResult<IEnumerable<TLogic>>> enumerateLogicDelegate, Func<TLogic, TModel> convertDelegate)
        {
            if (enumerateLogicDelegate != null)
            {
                ActionResult<IEnumerable<TLogic>> enumerationResult = enumerateLogicDelegate();
                return enumerationResult.IsValid ? ActionResult<IEnumerable<TModel>>.GetValidResult(enumerationResult.Result.Select(l => convertDelegate(l)).ToArray()) : ActionResult<IEnumerable<TModel>>.GetErrorResult(enumerationResult);
            }
            else
                return ActionResult<IEnumerable<TModel>>.GetErrorResult(new NotSupportedException());
        }
    }
}

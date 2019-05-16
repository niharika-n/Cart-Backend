using Omu.ValueInjecter;
using Omu.ValueInjecter.Injections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Common.CommonData;

namespace Common.Extentions
{
    public static class EntityMapperExtentions
    {
        public static List<TViewModel> MapFromModel<TModel, TViewModel>(this object target, List<TModel> model, string ignoreProperties = null)
            where TModel : class, new()
            where TViewModel : class, new()
        {
            return model.ToList().Select(o => new TViewModel().InjectFrom(new IgnoreProperty(ignoreProperties), o)).Cast<TViewModel>().ToList();
        }

        public static object MapFromModel<TModel>(this object target, TModel model, string ignoreProperties = null) where TModel : class, new()
        {
            target.InjectFrom(new IgnoreProperty(ignoreProperties), model);
            return target;
        }

        public static object MapFromViewModel<TModel>(this object target, TModel viewModel, ClaimsIdentity identity = null, string ignoreProperties = null) where TModel : class, new()
        {
            target.InjectFrom(new IgnoreProperty(ignoreProperties), viewModel)
                 .InjectFrom<AvoidNullProps>(viewModel);
            if (identity != null)
            {
                target.MapAuditColumns(identity);
            }
            return target;
        }
    }

    public class AvoidNullProps : LoopInjection
    {
        protected override void SetValue(object source, object target, PropertyInfo sp, PropertyInfo tp)
        {
            var val = sp.GetValue(source);
            if (val != null)
                tp.SetValue(target, val);
        }
    }

    public class IgnoreProperty : LoopInjection
    {
        private readonly string[] _ignoreProperties;

        public IgnoreProperty(string ignoreProperties)
        {
            if (ignoreProperties != null) { _ignoreProperties = ignoreProperties.Split(';'); }
        }
    }
}

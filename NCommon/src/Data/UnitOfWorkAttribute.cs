using System.Web.Mvc;

namespace NCommon.Data
{
    public class UnitOfWorkAttribute : ActionFilterAttribute
    {
        UnitOfWorkScope _unitOfWork;
        FilterScope _filterScope = FilterScope.Action;
        TransactionMode _transactionMode = TransactionMode.Default;

        public FilterScope Scope
        {
            get { return _filterScope; }
            set { _filterScope = value; }
        }

        public TransactionMode TransactionMode
        {
            get { return _transactionMode; }
            set { _transactionMode = value; }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //Starts a unit of work when the action starts executing.
            _unitOfWork = new UnitOfWorkScope(_transactionMode);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //Commits the transaction if the filter scope is action and no errors have occured.
            if (filterContext.Exception != null)
            {
                //Rollback...
                ReleaseUnitOfWork();
                return;
            }

            if (_filterScope != FilterScope.Action) 
                return;

            _unitOfWork.Commit();
            ReleaseUnitOfWork();
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            //Commits the unit of work if the filter scope is Result and no errors have occured.
            if (_filterScope != FilterScope.Result)
                return;
            _unitOfWork.Commit();
            ReleaseUnitOfWork();
        }

        void ReleaseUnitOfWork()
        {
            _unitOfWork.Dispose();
            _unitOfWork = null;
        }

        /// <summary>
        /// Defines the scope of the unit of work when executing in the context of an Action. Default is
        /// <see cref="Action"/>
        /// </summary>
        public enum FilterScope
        {
            /// <summary>
            /// Specifies that the unit of work scope will be comitted when the action finishes executing.
            /// </summary>
            Action,
            /// <summary>
            /// Specifies that the unit of work scope will be comitted when the view finishes rendering.
            /// </summary>
            Result
        }
    }
}
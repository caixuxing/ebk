using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace YueJia.Ebk.Application.Utils
{
    public static class DbTransaction
    {
        /// <summary>
        /// 事务事务
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="db"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<TResult> ExecuteInTransactionAsync<TResult>(ISqlSugarClient db, Func<Task<TResult>> action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {

            if (db is null) throw new ArgumentNullException(nameof(db), "数据库客户端不能为空");
            if (action is null) throw new ArgumentNullException(nameof(action), "事务执行的操作不能为空");
            try
            {
                await db.Ado.BeginTranAsync(isolationLevel);
                var result = await action();
                await db.Ado.CommitTranAsync();
                return result;
            }
            catch (Exception ex) when (ex is SqlException || ex is DbException)
            {
                await db.Ado.RollbackTranAsync();
                throw new InvalidOperationException($"事务执行失败: {ex.Message}", ex);
            }
        }
    }
}

namespace WT.Services.AccessToken.Repository
{
    public interface IRepository<T>
    {
        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        T Get();

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="t"></param>
        void Save(T t);

        /// <summary>
        /// 移除
        /// </summary>
        void Remove();
    }
}

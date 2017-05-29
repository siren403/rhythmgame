using UniRx;

namespace ScenePresenter
{
    public class SceneBase : PresenterBase
    {
        public object Argument { get; set; }
        public bool IsLoaded { get; set; }

        protected override void OnAwake()
        {
            this.InitializeAsObservable().Subscribe(_ => IsLoaded = true);
        }
        protected override IPresenter[] Children
        {
            get
            {
                return EmptyChildren;//Scene의 자식 Presenter들을 지정
            }
        }
        /// <summary>
        /// Call Sequence : Awake -> PrepareAsync -> BeforeInitialize -> Initialize
        /// </summary>
        /// <returns></returns>
        public virtual IObservable<Unit> PrepareAsync()
        {
            return Observable.Return(Unit.Default);
        }
        /// <summary>
        /// 상위 -> 하위
        /// </summary>
        protected override void BeforeInitialize() { }
        /// <summary>
        /// 하위 -> 상위
        /// </summary>
        protected override void Initialize() { }
    }

}
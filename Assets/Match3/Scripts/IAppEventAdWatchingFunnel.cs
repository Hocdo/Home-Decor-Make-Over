public interface IAppEventAdWatchingFunnel
{
	void SetAdCompletedStepReached(AppEventManager.AdCompletedStepReached step);

	void SendAppEventAdWatchingFunnel(int beforeLife, AppEventManager.AdCompletedStepReached step);
}

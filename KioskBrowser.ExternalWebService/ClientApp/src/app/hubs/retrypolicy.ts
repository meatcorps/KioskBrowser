import { IRetryPolicy, RetryContext } from "@microsoft/signalr";

export class RetryPolicy implements IRetryPolicy {
  nextRetryDelayInMilliseconds(retryContext: RetryContext): number | null {
    return 1000;
  }
}



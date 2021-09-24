//
//  iOSBridge.h
//  iOSBridge
//
//  Created by Supersonic.
//  Copyright (c) 2015 Supersonic. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <IronSource/IronSource.h>
static NSString *  UnityGitHash = @"c86a8d6";

@interface iOSBridge : NSObject<ISRewardedVideoDelegate,ISDemandOnlyRewardedVideoDelegate, ISInterstitialDelegate,ISDemandOnlyInterstitialDelegate, ISOfferwallDelegate, ISBannerDelegate, ISSegmentDelegate,ISImpressionDataDelegate, ISConsentViewDelegate>

@end



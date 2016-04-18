//
//  ScanViewController.h
//  TwinSprite
//
//  Created by Alfredo on 15/01/14.
//  Copyright (c) 2014 Carbonbyte. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <AVFoundation/AVFoundation.h>

@interface ScanViewController : UIViewController <AVCaptureMetadataOutputObjectsDelegate>

@property(nonatomic, retain) AVCaptureSession *session;
@property(nonatomic, retain) UIView *previewView;
@property(nonatomic, retain) AVCaptureVideoPreviewLayer *previewLayer;

@property (retain, nonatomic) IBOutlet UIView *previewContainer;
@property (retain, nonatomic) IBOutlet UIButton *button;

+ (NSString *) getCode;

- (IBAction)clickBack;

void show();

@end

//
//  ScanViewController.m
//  TwinSprite
//
//  Created by Alfredo
//  Copyright (c) 2014 Carbonbyte. All rights reserved.
//

#import "ScanViewController.h"
#import "iPhone_Common.h"


@implementation ScanViewController

static NSString *code = nil;

static NSString *objectName;
static NSString *functionName;

extern "C" {
	
    // Unity calls
	void _ScanQR (const char *objName, const char *fntName)
	{
        
        objectName = [[NSString alloc] initWithFormat: @"%s",objName];
        functionName = [[NSString alloc] initWithFormat: @"%s",fntName];
        
        // Get Window
        UIViewController *parentViewController = UnityGetGLViewController();
        
        // Make view controller
        ScanViewController *viewController = [[ScanViewController alloc] initWithNibName: @"ScanView" bundle: nil];
        
        
        // Add subview to view
        [parentViewController.view addSubview: viewController.view];
        
	}
}




+ (NSString *) getCode {
    return code;
}

- (IBAction) clickBack {
    [self.session stopRunning];
    [self.view removeFromSuperview];
    
    // Send result to Unity
    UnitySendMessage([objectName UTF8String], [functionName UTF8String], [@"CANCELLED" UTF8String]);
    
}


- (void)viewDidLoad {
    [super viewDidLoad];
    
    
    // Add rotate notification
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(rotateCallback:) name:UIDeviceOrientationDidChangeNotification object:nil];
    
    
    // Set code to nil
    code = nil;
    
    // Set full screen
    UIViewController *parentViewController = UnityGetGLViewController();
    self.view.frame = parentViewController.view.bounds;
    
    
    // Create a session
    self.session = [[AVCaptureSession alloc] init];
	
    // Create a preview layer
    
    // Rotate preview container
    if (UnityCurrentOrientation() == landscapeRight) {
        self.previewContainer.transform = CGAffineTransformMakeRotation(M_PI_2);
        self.previewContainer.frame = CGRectMake(0, 0, parentViewController.view.frame.size.width,  parentViewController.view.frame.size.height);
    } else if (UnityCurrentOrientation() == landscapeLeft) {
        self.previewContainer.transform = CGAffineTransformMakeRotation(-M_PI_2);
        self.previewContainer.frame = CGRectMake(0, 0, parentViewController.view.frame.size.width,  parentViewController.view.frame.size.height);
    }
    
    // Set preview
    self.previewLayer = [AVCaptureVideoPreviewLayer layerWithSession:self.session];
    self.previewLayer.frame = self.previewContainer.bounds;
    self.previewLayer.videoGravity = AVLayerVideoGravityResizeAspectFill;
    [self.previewContainer.layer addSublayer: self.previewLayer];
    
    
    
	// Get the Camera Device
    NSArray *devices = [AVCaptureDevice devicesWithMediaType:AVMediaTypeVideo];
	AVCaptureDevice *camera = nil;
    for(camera in devices) {
        if(camera.position == AVCaptureDevicePositionBack) {
            break;
        }
    }
	
	// Create a AVCaptureInput with the camera device
	NSError *error=nil;
	AVCaptureDeviceInput *cameraInput = [[AVCaptureDeviceInput alloc] initWithDevice:camera error:&error];
	if (cameraInput == nil) {
        // Send result to Unity
        UnitySendMessage([objectName UTF8String], [functionName UTF8String], [@"ERROR" UTF8String]);
        return;
	}
    
 	// Add the input and output
	[self.session addInput:cameraInput];
    
    // Create a VideoDataOutput and add it to the session
    AVCaptureMetadataOutput *output = [[AVCaptureMetadataOutput alloc] init];
    [self.session addOutput:output];
    
    
    // Add QR type
    NSSet *potentialDataTypes = [NSSet setWithArray:@[AVMetadataObjectTypeQRCode]];
    
    NSMutableArray *supportedMetaDataTypes = [NSMutableArray array];
    for(NSString *availableMetadataObject in output.availableMetadataObjectTypes) {
        if([potentialDataTypes containsObject:availableMetadataObject]) {
            [supportedMetaDataTypes addObject:availableMetadataObject];
        }
    }
    
    [output setMetadataObjectTypes:supportedMetaDataTypes];
    
    // Get called back when something is recognised
    [output setMetadataObjectsDelegate:self queue:dispatch_get_main_queue()];
	
	// Start the session running
	[self.session startRunning];
}

// Rotate callback
- (void) rotateCallback:(NSNotification *)notification {
    UIDeviceOrientation orientation = [[UIDevice currentDevice] orientation];
    
    UIViewController *parentViewController = UnityGetGLViewController();
    
    switch (orientation) {
        case landscapeRight:
            self.previewContainer.transform = CGAffineTransformMakeRotation(M_PI_2);
            self.previewContainer.frame = CGRectMake(0, 0, parentViewController.view.frame.size.width,  parentViewController.view.frame.size.height);
            break;
            
        case landscapeLeft:
            self.previewContainer.transform = CGAffineTransformMakeRotation(-M_PI_2);
            self.previewContainer.frame = CGRectMake(0, 0, parentViewController.view.frame.size.width,  parentViewController.view.frame.size.height);
            break;
            
       
    }
}


// Capture callback
- (void)captureOutput:(AVCaptureOutput *)captureOutput didOutputMetadataObjects:(NSArray *)metadataObjects fromConnection:(AVCaptureConnection *)connection {
    
    
    for(AVMetadataMachineReadableCodeObject *recognizedObject in metadataObjects) {
        
        // Stop session
        [self.session stopRunning];
        
        // Save code
        code = recognizedObject.stringValue;
        
        // Go back
        [self.navigationController popViewControllerAnimated: true];
        
        [self.session stopRunning];
        [self.view removeFromSuperview];
        
        // Send result to Unity
        UnitySendMessage([objectName UTF8String], [functionName UTF8String], [recognizedObject.stringValue UTF8String]);
    }
    
}



- (void) dealloc {
    
    [[NSNotificationCenter defaultCenter] removeObserver: self];
    
    [_session release];
    [_previewView release];
    [_previewLayer release];
    [_previewContainer release];
    
    
    [_previewContainer release];
    [_button release];
    [super dealloc];
}


@end

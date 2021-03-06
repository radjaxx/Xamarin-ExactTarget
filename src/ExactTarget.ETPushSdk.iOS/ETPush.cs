using System;
using MonoTouch.Foundation;
using MonoTouch.CoreLocation;
using MonoTouch.UIKit;

namespace ExactTarget.ETPushSdk
{
    /// <summary>
    /// Supporting protocol for OpenDirect, part of the ExactTarget 2013-02 release. 
    /// 
    /// Implementation of this delegate is not required for OpenDirect to function, but it is provided as a convenience to developers who do not with to parse the push payload on their own. 
    /// 
    /// All OpenDirect data is passed down as a JSON String, so you get it as an NSString. Please remember to parse it appropriately from there. Also, please remember to fail gracefully if you can't take action on the message. 
    /// 
    /// Also, please note that setting an OpenDirect Delegate will negate the automatic webpage loading feature added to MobilePush recently. This is deliberately to not stomp on your URLs and deep links. 
    /// </summary>
    [Model, Protocol, BaseType(typeof(NSObject))]
    public partial interface ExactTargetOpenDirectDelegate
    {
        /// <summary>
        /// Method called when an OpenDirect payload is received from MobilePush.
        /// </summary>
        /// <param name="payload">The contents of the payload as received from MobilePush.</param>
        [Export("didReceiveOpenDirectMessageWithContents:")]
        void DidReceiveOpenDirectMessageWithContents(string payload);

        /// <summary>
        /// Allows you to define the behavior of OpenDirect based on application state. 
        ///
        /// If set to YES, the OpenDirect delegate will be called if a push with an OpenDirect payload is received and the application state is running. This is counter to normal push behavior, so the default is NO.
        ///
        /// Consider that if you set this to YES, and the user is running the app when a push comes in, the app will start doing things that they didn't prompt it to do. This is bad user experience since it's confusing to the user. Along these lines, iOS won't present a notification if one is received while the app is running. 
        /// </summary>
        /// <value>
        /// representing whether or not you want action to be taken.
        /// </value>
        [Export("shouldDeliverOpenDirectMessageIfAppIsRunning")]
        bool ShouldDeliverOpenDirectMessageIfAppIsRunning { get; }
    }

    /// <summary>
    /// This is the main interface to the ExactTarget MobilePush SDK. It is meant to handle a lot of the heavy lifting with regards to sending data back to ExactTarget.
    /// Please note that this is a singleton object, and you should reference it as [ETPush pushManager].
    /// </summary>
    [BaseType(typeof(NSObject))]
    [DisableDefaultCtor]
    public partial interface ETPush
    {
        // NSDate *_sessionStart;
        // NSString *_messageID;

        // BOOL _showLocalAlert;

        // // OpenDirect Delegate stuff
        // id<ExactTargetOpenDirectDelegate> _openDirectDelegate;

        #region Configure the app for ETPush

        /// <summary>
        /// Returns (or initializes) the shared pushManager instance.
        /// </summary>
        /// <value>
        /// The singleton instance of an ETPush pushManager.
        /// </value>
        [Static, Export("pushManager")]
        ETPush PushManager { get; }

        /// <summary>
        /// Returns (or initializes) the shared pushManager instance.
        /// </summary>
        /// <value>
        /// The singleton instance of an ETPush pushManager.
        /// </value>
        //[Export("init")]
        //IntPtr Constructor();

        /// <summary>
        /// This is the former main configuration for the MobilePush SDK. As of version 3.0, it is succeeded by configureSDKWithAppID:andAccessToken:withAnalytics:andLocationServices:andCloudPages:. It will continue to function, but calls it's successor with YES for all parameters. This may provide undesired results, so you are encouraged to switch your configuration method to the new one in your next release.
        /// </summary>
        /// <param name="etAppID">The App ID generated by Code@ExactTarget to identify the consumer app.</param>
        /// <param name="accessToken">The designed token given to you by Code@ExactTarget that allows you access to the API.</param>
        [Obsolete]
        [Export("configureSDKWithAppID:andAccessToken:")]
        void ConfigureSDKWithAppID(string etAppID, string accessToken);

        /// <summary>
        /// This is the main configuration method, responsible for setting credentials needed to communicate with ExactTarget. If you are unsure of your accessToken or environment, please visit Code@ExactTarget
        /// 
        /// Each of the flags in the method are used to control various aspects of the MobilePush SDK. The act as global on/off switches, meaning that if you disable one here, it is off eveywhere.
        /// </summary>
        /// <param name="etAppID">The App ID generated by Code@ExactTarget to identify the consumer app.</param>
        /// <param name="accessToken">The designed token given to you by Code@ExactTarget that allows you access to the API.</param>
        /// <param name="analyticsState">Whether or not to send analytic data back to ExactTarget.</param>
        /// <param name="locState">Whether or not to use Location Services.</param>
        /// <param name="cpState">Whether or not to use Cloud Pages.</param>
        [Export("configureSDKWithAppID:andAccessToken:withAnalytics:andLocationServices:andCloudPages:")]
        void ConfigureSDKWithAppID(string etAppID, string accessToken, bool analyticsState, bool locState, bool cpState);

        /// <summary>
        /// Sets the OpenDirect delegate.
        /// </summary>
        /// <param name="delegate_">The object you wish to be called when an OpenDirect message is delivered.</param>
        [Export("setOpenDirectDelegate:")]
        void setOpenDirectDelegate(ExactTargetOpenDirectDelegate delegate_);

        /// <summary>
        /// Returns the OpenDirect delegate.
        /// </summary>
        /// <value>
        /// The open direct delegate.
        /// </value>
        [Export("openDirectDelegate")]
        ExactTargetOpenDirectDelegate OpenDirectDelegate { get; }

        /// <summary>
        /// Triggers a data send to ExactTarget. Mostly used internally, and rarely should be called by client code.
        /// </summary>
        [Export("updateET")]
        void UpdateET();

        #endregion

        // These methods are required to make push function on iOS, and to enable the ET SDK to utilize it. All of these methods are required.
        #region Push Lifecycle
        // Refer to Availability.h for the reasoning behind why the following #if's are used. Basically, this will allow the code to be compiled for different IPHONEOS_DEPLOYMENT_TARGET values to
        // maintain backward compatibility for running on IOS 6.0 and up as well allowing for using different versions of the IOS SDK compiled using XCode 5.X, XCode 6.X and up without getting depricated warnings or undefined warnings.

#if true

        /// <summary>
        /// Wrapper for iOS' application:registerForRemoteNotification; call. It will check that push is allowed, and if so, register with Apple for a token. If push is not enabled, it will notify ExactTarget that push is disabled.
        /// </summary>
        [Export("registerForRemoteNotifications")]
        void RegisterForRemoteNotifications();

        /// <summary>
        /// Wrapper for iOS' isRegisteredForRemoteNotifications; call.
        /// </summary>
        /// <value>
        /// <c>true</c> if [is registered for remote notifications]; otherwise, <c>false</c>.
        /// </value>
        [Export("isRegisteredForRemoteNotifications")]
        bool IsRegisteredForRemoteNotifications { get; }

        /// <summary>
        /// Wrapper for iOS' application:registerUserNotificationSettings; call.
        /// </summary>
        /// <param name="notificationSettings">The UIUserNotificationSettings object that the application would like to use for push. These are pipe-delimited, and use Apple's native values.</param>
        [Export("registerUserNotificationSettings:")]
        void RegisterUserNotificationSettings(UIUserNotificationSettings notificationSettings);

        /// <summary>
        /// Wrapper for iOS' currentUserNotificationSettings; call.
        /// </summary>
        /// <value>
        /// The current user notification settings.
        /// </value>
        [Export("currentUserNotificationSettings")]
        UIUserNotificationSettings CurrentUserNotificationSettings { get; }

        /// <summary>
        /// Wrapper for iOS' didRegisterUserNotificationSettings; callback.
        /// </summary>
        /// <param name="notificationSettings">The notification settings.</param>
        [Export("didRegisterUserNotificationSettings:")]
        void DidRegisterUserNotificationSettings(UIUserNotificationSettings notificationSettings);

#endif

        /// <summary>
        /// Wrapper for iOS' application:registerForRemoteNotificationTypes; call. It will check that push is allowed, and if so, register with Apple for a token. If push is not enabled, it will notify ExactTarget that push is disabled.
        /// </summary>
        /// <param name="types">The UIRemoteNotificationTypes that the application would like to use for push. These are pipe-delimited, and use Apple's native values.</param>
        [Export("registerForRemoteNotificationTypes:")]
        void RegisterForRemoteNotificationTypes(UIRemoteNotificationType types);

        /// <summary>
        /// Responsible for sending a received token back to ExactTarget. It marks the end of the token registration flow. If it is unable to reach ET server, it will save the token and try again later. 
        /// 
        /// This method is necessary to implementation of ET Push.
        /// </summary>
        /// <param name="deviceToken">Token as received from Apple, still an NSData object.</param>
        [Export("registerDeviceToken:")]
        void RegisterDeviceToken(NSData deviceToken);

        /// <summary>
        /// Returns the device token as a NSString. As requested via GitHub (Issue #3).
        /// </summary>
        /// <value>
        /// A stringified version of the Device Token.
        /// </value>
        [Export("deviceToken")]
        string DeviceToken { get; }

        /// <summary>
        /// Handles a registration failure.
        /// </summary>
        /// <param name="error">The error returned to the application on a registration failure.</param>
        [Export("applicationDidFailToRegisterForRemoteNotificationsWithError:")]
        void ApplicationDidFailToRegisterForRemoteNotificationsWithError(NSError error);

        /// <summary>
        /// Reset the application's badge number to zero (aka, remove it) and let the push servers know that it should zero the count.
        /// </summary>
        [Export("resetBadgeCount")]
        void ResetBadgeCount();

        /// <summary>
        /// Tell the SDK to display a UIAlertView if a push is received while the app is already running. Default behavior is set to NO.
        /// 
        /// Please note that all push notifications received by the application will be processed, but iOS will *not* present an alert to the user if the app is running when the alert is received. If you set this value to true (YES), then the SDK will generate and present the alert for you. It will not play a sound, though.
        /// </summary>
        /// <param name="desiredState">YES/NO if you want to display an alert view while the app is running.</param>
        [Export("shouldDisplayAlertViewIfPushReceived:")]
        void ShouldDisplayAlertViewIfPushReceived(bool desiredState);

        #endregion

        // These methods are not necessary for the Push lifecycle, but are required to make the ET Push SDK perform as expected
        #region Application Lifecycle

        /// <summary>
        /// Notifies the ET SDK of an app launch, including the dictionary sent to the app by iOS. The launchOptions dictionary is necessary because it will include the APNS dictionary, necessary for processing opens and other analytic information. 
        /// </summary>
        /// <param name="launchOptions">The dictionary passed to the application by iOS on launch.</param>
        [Export("applicationLaunchedWithOptions:")]
        void ApplicationLaunchedWithOptions(NSDictionary launchOptions);

        /// <summary>
        /// Notifies the ET SDK of an app termination. Internally, this method does a lot of cleanup. 
        /// </summary>
        [Export("applicationTerminated")]
        void ApplicationTerminated();

        /// <summary>
        /// Handles a push notification received by the app when the application is already running. 
        /// 
        /// This method must be implemented in [[UIApplication sharedApplication] didReceiveRemoteNotification:userInfo].
        /// 
        /// Sometimes, when a push comes in, the application will already be running (it happens). This method rises to the occasion of handing that notification, displaying an Alert (if the SDK is configured to do so), and calling all of the analytic methods that wouldn't be called otherwise. 
        /// </summary>
        /// <param name="userInfo">The dictionary containing the push notification.</param>
        /// <param name="applicationState">State of the application at time of notification.</param>
        [Export("handleNotification:forApplicationState:")]
        void HandleNotification(NSDictionary userInfo, UIApplicationState applicationState);

        /// <summary>
        /// Handles a local notification received by the application. 
        /// 
        /// Sometimes the SDK will use local notifications to indicate something to the user. These are handled differently by iOS, and as such, need to be implemented differently in the SDK. Sorry about that. 
        /// </summary>
        /// <param name="notification">The received UILocalNotification.</param>
        [Export("handleLocalNotification:")]
        void HandleLocalNotification(UILocalNotification notification);

        #endregion

        #region Data Interaction

        /// <summary>
        /// Accepts and sets the Subscriber Key for the device's user.
        /// </summary>
        /// <param name="subscriberKey">The subscriber key to attribute to the user.</param>
        [Export("setSubscriberKey")]
        void SetSubscriberKey(string subscriberKey);

        /// <summary>
        /// Returns the subscriber key for the active user, in case you need it.
        /// </summary>
        /// <value>
        /// The get subscriber key.
        /// </value>
        [Export("getSubscriberKey")]
        string GetSubscriberKey { get; }

        /// <summary>
        /// Adds the tag.
        /// </summary>
        /// <param name="tag">A string to add to the list of tags.</param>
        [Export("addTag:")]
        void AddTag(string tag);

        /// <summary>
        /// Removes the provided Tag (NSString) from the list of tags.
        /// </summary>
        /// <param name="tag">A string to remove from the list of tags.</param>
        /// <returns>Echoes the tag back on successful removal, or nil if something failed.</returns>
        [Export("removeTag:")]
        string RemoveTag(string tag);

        /// <summary>
        /// Returns the list of tags for this device.
        /// </summary>
        /// <value>
        /// All tags.
        /// </value>
        [Export("allTags")]
        NSSet AllTags { get; }

        /// <summary>
        /// Adds an attribute to the data set sent to ExactTarget.
        /// </summary>
        /// <param name="name">The name of the attribute you wish to send. This will be the key of the pair..</param>
        /// <param name="value">The value to set for thid data pair.</param>
        [Export("addAttributeNamed:value:")]
        void AddAttributeNamed(string name, string value);

        /// <summary>
        /// Removes the provided attributef rom the data set to send to ExactTarget.
        /// </summary>
        /// <param name="name">The name of the attribute you wish to remove. .</param>
        /// <returns>Returns the value that was set. It will no longer be sent back to ExactTarget.</returns>
        [Export("removeAttributeNamed:")]
        string RemoveAttributeNamed(string name);

        /// <summary>
        /// Returns a read-only copy of the Attributes dictionary as it is right now. 
        /// </summary>
        /// <value>
        /// All attributes currently set.
        /// </value>
        [Export("allAttributes")]
        NSDictionary AllAttributes { get; }

        #endregion

        #region ETPush Convenience Methods

        /// <summary>
        /// Gets the Apple-safe, unique Device Identifier that ET will later use to identify the device.
        /// 
        /// Note that this method is compliant with Apple's compliance rules, but may not be permanent.
        /// </summary>
        /// <value>
        /// The safe device identifier.
        /// </value>
        [Static, Export("safeDeviceIdentifier")]
        string SafeDeviceIdentifier { get; }

        /// <summary>
        /// Returns the hardware identification string, like "iPhone1,1". ExactTarget uses this data for segmentation. 
        /// </summary>
        /// <value>
        /// A string of the hardware identification.
        /// </value>
        [Static, Export("hardwareIdentifier")]
        string HardwareIdentifier { get; }

        /// <summary>
        /// Returns the state of Push based on logic reflected at ExactTarget. 
        /// </summary>
        /// <value>
        ///   As of this release, Push is considered enabled if the application is able to present an alert (banner, alert) to the user per Settings. Nothing else will be considered.
        /// </value>
        [Static, Export("isPushEnabled")]
        bool IsPushEnabled { get; }

        #endregion

        #region Listeners for UIApplication events

        /// <summary>
        /// Sets up the listeners.
        /// </summary>
        [Export("startListeningForApplicationNotifications")]
        void StartListeningForApplicationNotifications();

        /// <summary>
        /// Drops the listeners.
        /// </summary>
        [Export("stopListeningForApplicationNotifications")]
        void StopListeningForApplicationNotifications();

        /// <summary>
        /// Responds to the UIApplicationDidBecomeActiveNotification notification
        /// </summary>
        [Export("applicationDidBecomeActiveNotificationReceived")]
        void ApplicationDidBecomeActiveNotificationReceived(); // UIApplicationDidBecomeActiveNotification

        /// <summary>
        /// Responds to the UIApplicationDidEnterBackgroundNotification notification
        /// </summary>
        [Export("applicationDidEnterBackgroundNotificationReceived")]
        void ApplicationDidEnterBackgroundNotificationReceived(); // UIApplicationDidEnterBackgroundNotification

        /// <summary>
        /// Set the Log Level
        /// </summary>
        /// <param name="state">if set to <c>true</c> [state].</param>
        [Static, Export("setETLoggerToRequiredState:")]
        void setETLoggerToRequiredState(bool state);

        /// <summary>
        /// To Log the string whenever [ETPush setETLoggerToState:YES]
        /// </summary>
        /// <param name="stringToBeLogged">The string to be logged.</param>
        [Static, Export("ETLogger:")]
        void setETLoggerToRequiredState(string stringToBeLogged);
        
        #endregion
    }
}

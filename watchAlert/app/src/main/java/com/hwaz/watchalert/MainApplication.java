package com.hwaz.watchalert;
import android.app.Application;
import cn.jpush.android.api.JPushInterface;

public class MainApplication extends Application {
    @Override
    public void onCreate() {
        super.onCreate();
        JPushInterface.setDebugMode(true);
        JPushInterface.init(this);
    }
}

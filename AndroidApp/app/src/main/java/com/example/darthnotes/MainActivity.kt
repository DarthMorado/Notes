package com.example.darthnotes

import android.annotation.SuppressLint
import android.content.Intent
import android.net.Uri
import android.os.Bundle
import android.webkit.CookieManager
import android.webkit.WebResourceRequest
import android.webkit.WebView
import android.webkit.WebViewClient
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.browser.customtabs.CustomTabsIntent
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.safeDrawingPadding
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.viewinterop.AndroidView

class MainActivity : ComponentActivity() {

    companion object {
        var webView: WebView? = null
    }

    @SuppressLint("SetJavaScriptEnabled")
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        handleDeepLink(intent)

        setContent {
            WebPage()
        }
    }

    override fun onNewIntent(intent: Intent) {
        super.onNewIntent(intent)

        handleDeepLink(intent)
    }

    private fun handleDeepLink(intent: Intent?) {
        val data: Uri? = intent?.data



        if (
            data != null &&
            data.scheme == "darthnotes" &&
            data.host == "auth-success"
        ) {
            val token = data.getQueryParameter("token")
            if (!token.isNullOrEmpty()) {
                webView?.post {
                    CookieManager.getInstance().flush()
                    webView?.loadUrl("https://notes.darth.lv/Auth/LoginByOTP?otp={"+token+"}")
                }
            }
            else {

                webView?.post {
                    CookieManager.getInstance().flush()
                    webView?.loadUrl("https://notes.darth.lv")
                }
            }
        }
    }
}

@Composable
fun WebPage() {

    AndroidView(
        modifier = Modifier
            .fillMaxSize()
            .safeDrawingPadding(),

        factory = { context ->

            WebView(context).apply {

                MainActivity.webView = this

                settings.javaScriptEnabled = true
                settings.domStorageEnabled = true

                val cookieManager = CookieManager.getInstance()

                cookieManager.setAcceptCookie(true)

                cookieManager.setAcceptThirdPartyCookies(
                    this,
                    true
                )

                webViewClient = object : WebViewClient() {

                    override fun shouldOverrideUrlLoading(
                        view: WebView?,
                        request: WebResourceRequest?
                    ): Boolean {

                        val url = request?.url.toString()

                        if (url.lowercase().contains("test"))
                        {
                            val customTabsIntent =
                                CustomTabsIntent.Builder()
                                    .build()

                            customTabsIntent.launchUrl(
                                context,
                                Uri.parse(url)
                                    .buildUpon()
                                    .build()
                            )

                            return true
                        }

                        if (url.contains("GoogleLogin")) {

                            val loginUrl = Uri.parse(url)
                                .buildUpon()
                                .appendQueryParameter(
                                    "isForApp",
                                    "true"
                                )
                                .build()

                            val customTabsIntent =
                                CustomTabsIntent.Builder()
                                    .build()

                            customTabsIntent.launchUrl(
                                context,
                                loginUrl
                            )

                            return true
                        }

                        return false
                    }
                }

                loadUrl("https://notes.darth.lv")
            }
        }
    )
}
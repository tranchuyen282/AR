package com.e.augimage;

import androidx.appcompat.app.AppCompatActivity;

import android.Manifest;
import com.google.ar.sceneform.ux.ArFragment;
import android.os.Bundle;
import android.view.View;
import android.widget.TextView;
import android.widget.Toast;

import com.google.ar.core.Anchor;
import com.google.ar.core.AugmentedImage;
import com.google.ar.core.AugmentedImageDatabase;
import com.google.ar.core.Config;
import com.google.ar.core.Frame;
import com.google.ar.core.Session;
import com.google.ar.core.TrackingState;
import com.google.ar.core.exceptions.CameraNotAvailableException;
import com.google.ar.core.exceptions.UnavailableApkTooOldException;
import com.google.ar.core.exceptions.UnavailableArcoreNotInstalledException;
import com.google.ar.core.exceptions.UnavailableDeviceNotCompatibleException;
import com.google.ar.core.exceptions.UnavailableSdkTooOldException;
import com.google.ar.sceneform.AnchorNode;
import com.google.ar.sceneform.ArSceneView;
import com.google.ar.sceneform.FrameTime;
import com.google.ar.sceneform.Node;
import com.google.ar.sceneform.Scene;
import com.google.ar.sceneform.math.Quaternion;
import com.google.ar.sceneform.math.Vector3;
import com.google.ar.sceneform.rendering.ModelRenderable;
import com.google.ar.sceneform.rendering.ViewRenderable;
import com.google.ar.sceneform.ux.ArFragment;
import com.google.ar.sceneform.ux.TransformableNode;
import com.karumi.dexter.Dexter;
import com.karumi.dexter.PermissionToken;
import com.karumi.dexter.listener.PermissionDeniedResponse;
import com.karumi.dexter.listener.PermissionGrantedResponse;
import com.karumi.dexter.listener.PermissionRequest;
import com.karumi.dexter.listener.single.PermissionListener;

import org.w3c.dom.Text;

import java.io.IOException;
import java.io.InputStream;
import java.util.Collection;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ExecutionException;

public class MainActivity extends AppCompatActivity implements Scene.OnUpdateListener {

    private ArSceneView arView;
    private ArFragment arFragment;
    private Session session;
    private boolean shouldConfigureSession = false;
//    private ViewRenderable textRenderable;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        //view
        //arView = findViewById(R.id.arView);
        arFragment = (ArFragment) getSupportFragmentManager().findFragmentById(R.id.arView);
        arView = arFragment.getArSceneView();

        // request permission

        Dexter.withActivity(this)
                .withPermission(Manifest.permission.CAMERA)
                .withListener(new PermissionListener() {
                    @Override
                    public void onPermissionGranted(PermissionGrantedResponse response) {
                        setupSession();
                    }

                    @Override
                    public void onPermissionDenied(PermissionDeniedResponse response) {
                        Toast.makeText(MainActivity.this, "permission need to display camera", Toast.LENGTH_LONG).show();
                    }

                    @Override
                    public void onPermissionRationaleShouldBeShown(PermissionRequest permission, PermissionToken token) {

                    }
                }).check();

        //Build a renderable from a 2D View.
//        CompletableFuture<ViewRenderable> textView =
//                ViewRenderable.builder().setView(this, R.layout.name_animal).build();
//
//        CompletableFuture.allOf(textView)
//                .handle(
//                        (notUsed, throwable) -> {
//
//                            if (throwable != null) {
//                                return null;
//                            }
//                            try {
//                                textRenderable = textView.get();
//                            } catch (InterruptedException | ExecutionException ex) {
//
//                            }
//
//                            return null;
//                        });

        initSceneView();
    }

    private void initSceneView() {
        arView.getScene().addOnUpdateListener(this);
    }

    private void setupSession() {
        if (session == null) {
            try {
                session = new Session(this);
            } catch (UnavailableArcoreNotInstalledException e) {
                e.printStackTrace();
            } catch (UnavailableApkTooOldException e) {
                e.printStackTrace();
            } catch (UnavailableSdkTooOldException e) {
                e.printStackTrace();
            } catch (UnavailableDeviceNotCompatibleException e) {
                e.printStackTrace();
            }

            shouldConfigureSession = true;

            if (shouldConfigureSession) {
                configSession();
                shouldConfigureSession = false;
                arView.setupSession(session);
            }

            try {
                session.resume();
                arView.resume();
            } catch (CameraNotAvailableException e) {
                e.printStackTrace();
                session = null;
                return;
            }

        }
    }

    private void configSession() {
        Config config = new Config(session);
        if (!buildDatabase(config)) {
            Toast.makeText(this, "Lỗi Database", Toast.LENGTH_LONG).show();
        }
        config.setUpdateMode(Config.UpdateMode.LATEST_CAMERA_IMAGE);
        session.configure(config);
    }

    private boolean buildDatabase(Config config) {
        AugmentedImageDatabase augmentedImageDatabase;
        try {
            InputStream is = getAssets().open("database.imgdb");
            augmentedImageDatabase = AugmentedImageDatabase.deserialize(session, is);
            config.setAugmentedImageDatabase(augmentedImageDatabase);
            session.configure(config);
            return true;
        } catch (IOException e) {
            e.printStackTrace();
            return false;
        }

    }


    @Override
    public void onUpdate(FrameTime frameTime) {
        Frame frame = arView.getArFrame();
        Collection<AugmentedImage> updateAugmentedImg = frame.getUpdatedTrackables(AugmentedImage.class);



        for (AugmentedImage image : updateAugmentedImg) {

            //&& image.getTrackingMethod() == AugmentedImage.TrackingMethod.FULL_TRACKING
            if (image.getTrackingState() == TrackingState.TRACKING ) {
                String name = image.getName();
                if(image.getIndex() == 1){
                    AnchorNode node = new AnchorNode();
                    node.setAnchor(image.createAnchor(image.getCenterPose()));
                    createTextView(node, "Vinacafe 45k/pcs",image);
                    //node.addChild(nodeTextView);
                    //Toast.makeText(MainActivity.this,String.valueOf(image.getCenterPose().tx()) +" "+ String.valueOf(image.getCenterPose().ty())+ " "+String.valueOf(image.getCenterPose().tz()), Toast.LENGTH_LONG).show();
                    node.setSmoothed(true);
                }else if(image.getIndex() == 2){
                    AnchorNode node1 = new AnchorNode();
                    arView.getScene().addChild(node1);
                    node1.setAnchor(image.createAnchor(image.getCenterPose()));
                    createTextView(node1, "lion 45k/pcs",image);
                    // node1.addChild(textView1);
                    //Toast.makeText(MainActivity.this,"lion", Toast.LENGTH_LONG).show();
                    node1.setSmoothed(true);
                }
            }


        }
    }

    private void createTextView(AnchorNode node, String name, AugmentedImage image) {
        ViewRenderable.builder().setView(this,R.layout.name_animal)
                .build()
                .thenAccept(viewRenderable -> {
                    TransformableNode textView= new TransformableNode(arFragment.getTransformationSystem());
                    textView.setRenderable(viewRenderable);
                    textView.setLocalPosition(new Vector3(0.0f, 0.0f, -0.5f * image.getExtentZ()));
                    textView.setLocalRotation(new Quaternion(new Vector3(1.0f, 0.0f, 0.0f),-90));
                    TextView txtName = (TextView) viewRenderable.getView();
                    txtName.setText(name);
                    textView.setParent(node);
                });
    }

    @Override
    protected void onResume() {
        super.onResume();
        Dexter.withActivity(this)
                .withPermission(Manifest.permission.CAMERA)
                .withListener(new PermissionListener() {
                    @Override
                    public void onPermissionGranted(PermissionGrantedResponse response) {
                        setupSession();
                    }

                    @Override
                    public void onPermissionDenied(PermissionDeniedResponse response) {
                        Toast.makeText(MainActivity.this, "permission need to display camera", Toast.LENGTH_LONG).show();
                    }

                    @Override
                    public void onPermissionRationaleShouldBeShown(PermissionRequest permission, PermissionToken token) {

                    }
                }).check();
    }

    @Override
    protected void onPause() {
        super.onPause();
        if (session != null) {
            arView.pause();
            session.pause();
        }
    }
}

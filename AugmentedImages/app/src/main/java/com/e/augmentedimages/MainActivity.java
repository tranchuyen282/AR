package com.e.augmentedimages;

import androidx.appcompat.app.AppCompatActivity;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.os.Bundle;

import com.google.ar.core.Anchor;
import com.google.ar.core.AugmentedImage;
import com.google.ar.core.AugmentedImageDatabase;
import com.google.ar.core.Config;
import com.google.ar.core.Frame;
import com.google.ar.core.Session;
import com.google.ar.core.TrackingState;
import com.google.ar.sceneform.AnchorNode;
import com.google.ar.sceneform.FrameTime;
import com.google.ar.sceneform.Scene;
import com.google.ar.sceneform.rendering.ModelRenderable;

import java.util.Collection;

public class MainActivity extends AppCompatActivity implements Scene.OnUpdateListener{

    private  CustomArFrag arFragment;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        arFragment = (CustomArFrag) getSupportFragmentManager().findFragmentById(R.id.fragment);
    }

    public void setupDatabase(Config config, Session session){
        Bitmap foxBitmap = BitmapFactory.decodeResource(getResources(),R.drawable.fox);
        AugmentedImageDatabase aid = new AugmentedImageDatabase(session);
        aid.addImage("fox", foxBitmap);
        config.setAugmentedImageDatabase(aid);
    }

    @Override
    public void onUpdate(FrameTime frameTime) {
        Frame frame = arFragment.getArSceneView().getArFrame();
        Collection<AugmentedImage> images = frame.getUpdatedTrackables(AugmentedImage.class);

        for (AugmentedImage image: images){
            if(image.getTrackingState() == TrackingState.TRACKING){
                if (image.getName().equals("fox")){

                    Anchor anchor = image.createAnchor(image.getCenterPose());

                    creatModel(anchor);
                }
            }
        }
    }

    private void creatModel(Anchor anchor) {
        ModelRenderable.builder()
                .setSource(this, Uri.parse("ArcticFox_Posed.sfb"))
                .build()
                .thenAccept(modelRenderable -> placeModel(modelRenderable,anchor));
    }

    private void placeModel(ModelRenderable modelRenderable, Anchor anchor) {
        AnchorNode anchorNode = new AnchorNode(anchor);
        anchorNode.setRenderable(modelRenderable);
        arFragment.getArSceneView().getScene().addChild(anchorNode);
    }
}

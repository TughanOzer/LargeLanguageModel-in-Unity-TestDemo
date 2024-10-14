using System;
using System.IO;

public class ImageToBase64 {
    public static string ConvertImageToBase64(string imagePath) {
        byte[] imageBytes = File.ReadAllBytes(imagePath);
        return Convert.ToBase64String(imageBytes);
    }
}
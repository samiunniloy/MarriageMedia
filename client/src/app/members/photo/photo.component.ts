import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';

@Component({
  selector: 'app-photo',
  standalone:true,
  imports: [CommonModule],
  templateUrl: './photo.component.html',
  styleUrl: './photo.component.css'
})
export class PhotoComponent {
  selectedImage: string | ArrayBuffer | null = null;
  images: any[] = [];
  isUploadMode: boolean = true; 

  constructor(private http: HttpClient) { }

  setMode(mode: string) {
    this.isUploadMode = mode === 'upload';
    if (!this.isUploadMode) {
      this.fetchImages();
    }
  }

  onFileChange(event: any) {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => {
        this.selectedImage = reader.result;
      };
    }
  }

  uploadImage() {
    if (this.selectedImage) {
      const formData = new FormData();
      const base64Data = this.selectedImage.toString();

      const imagePayload = {
        imageName: 'example-image',
        base64Image: base64Data
      };

      this.http.post('http://localhost:5126/api/images', imagePayload, {
        headers: {
          'Content-Type': 'application/json'
        }
      }).subscribe(
        (response) => {
          console.log('Image uploaded successfully!', response);
          alert('Image uploaded successfully!');
          this.selectedImage = null; 
        },
        (error) => {
          console.error('Error uploading image', error);
          alert('Error uploading image');
        }
      );
    } else {
      alert('Please select an image first.');
    }
  }

  fetchImages() {
    this.http.get<any[]>('http://localhost:5126/api/images').subscribe(
      (response) => {
        this.images = response.map(img => ({
          ...img,
          displayUrl: 'data:image/jpeg;base64,' + this.arrayBufferToBase64(img.imageData)
        }));
      },
      (error) => {
        console.error('Error fetching images', error);
      }
    );
  }

  private arrayBufferToBase64(buffer: number[]): string {
    const binary = String.fromCharCode.apply(null, buffer);
    return window.btoa(binary);
  }
}

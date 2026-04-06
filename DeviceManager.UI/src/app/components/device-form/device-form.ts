import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { DeviceService } from '../../services/device';
import { Device } from '../../models/device.model';

@Component({
  selector: 'app-device-form',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './device-form.html',
  styleUrl: './device-form.css',
})
export class DeviceForm implements OnInit {
  device: Device = {
    id: 0,
    name: '',
    manufacturer: '',
    type: '',
    operatingSystem: '',
    osVersion: '',
    processor: '',
    ram: 0,
    description: null,
    userId: null,
    user: null
  };

  isEditMode = false;
  submitted = false;
  isGenerating = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private deviceService: DeviceService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.deviceService.getDevice(Number(id)).subscribe({
        next: (data) => {
          this.device = data;
          this.cdr.detectChanges();
        },
        error: (err) => console.error('Error loading device:', err)
      });
    }
  }

  generateDescription(): void {
    if (!this.device.name || !this.device.manufacturer || !this.device.type ||
        !this.device.operatingSystem || !this.device.processor || !this.device.ram) {
      alert('Please fill in Name, Manufacturer, Type, OS, Processor, and RAM before generating a description.');
      return;
    }

    this.isGenerating = true;

    this.deviceService.generateDescription({
      name: this.device.name,
      manufacturer: this.device.manufacturer,
      type: this.device.type,
      operatingSystem: this.device.operatingSystem,
      processor: this.device.processor,
      ram: this.device.ram
    }).subscribe({
      next: (response) => {
        this.device.description = response.description;
        this.isGenerating = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Error generating description:', err);
        alert('Failed to generate description.');
        this.isGenerating = false;
        this.cdr.detectChanges();
      }
    });
  }

  save(): void {
    this.submitted = true;

    if (!this.device.name || !this.device.manufacturer || !this.device.type ||
        !this.device.operatingSystem || !this.device.osVersion ||
        !this.device.processor || !this.device.ram) {
      return;
    }

    if (this.isEditMode) {
      this.deviceService.updateDevice(this.device.id, this.device).subscribe({
        next: () => this.router.navigate(['/devices']),
        error: (err) => console.error('Error updating device:', err)
      });
    } else {
      this.deviceService.getDevices().subscribe({
        next: (devices) => {
          const exists = devices.some(
            d => d.name.toLowerCase() === this.device.name.toLowerCase() &&
                 d.manufacturer.toLowerCase() === this.device.manufacturer.toLowerCase()
          );

          if (exists) {
            alert('A device with this name and manufacturer already exists!');
            return;
          }

          this.deviceService.createDevice(this.device).subscribe({
            next: () => this.router.navigate(['/devices']),
            error: (err) => console.error('Error creating device:', err)
          });
        }
      });
    }
  }
}
import { Component, OnInit } from '@angular/core';
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

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private deviceService: DeviceService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.deviceService.getDevice(Number(id)).subscribe({
        next: (data) => this.device = data,
        error: (err) => console.error('Error loading device:', err)
      });
    }
  }

  save(): void {
    this.submitted = true;

    if (!this.device.name || !this.device.manufacturer || !this.device.type || !this.device.operatingSystem ||
       !this.device.osVersion || !this.device.processor || !this.device.ram) {
      return;
    }

    if (this.isEditMode) {
      this.deviceService.updateDevice(this.device.id, this. device).subscribe({
        next: () => this.router.navigate(['/devices']),
        error: (err) => console.error('Error updating device:', err)
      });
    } else {
      this.deviceService.createDevice(this.device).subscribe({
        next: () => this.router.navigate(['/devices']),
        error: (err) => console.error('Error creating device:', err)
      });
    }
  }
}
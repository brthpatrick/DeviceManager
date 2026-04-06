import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { DeviceService } from '../../services/device';
import { AuthService } from '../../services/auth';
import { Device } from '../../models/device.model';

@Component({
  selector: 'app-device-list',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './device-list.html',
  styleUrl: './device-list.css',
})
export class DeviceList implements OnInit {
  devices: Device[] = [];
  searchQuery = '';

  constructor(
    private deviceService: DeviceService,
    public authService: AuthService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadDevices();
  }

  loadDevices(): void {
    this.deviceService.getDevices().subscribe({
      next: (data) => {
        this.devices = data;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error loading devices:', err)
    });
  }

  search(): void {
    if (!this.searchQuery.trim()) {
      this.loadDevices();
      return;
    }

    this.deviceService.searchDevices(this.searchQuery).subscribe({
      next: (data) => {
        this.devices = data;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Error searching devices:', err)
    });
  }

  clearSearch(): void {
    this.searchQuery = '';
    this.loadDevices();
  }

  deleteDevice(id: number): void {
    if (confirm('Are you sure you want to delete this device?')) {
      this.deviceService.deleteDevice(id).subscribe({
        next: () => {
          if (this.searchQuery.trim()) {
            this.search();
          } else {
            this.loadDevices();
          }
        },
        error: (err) => console.error('Error deleting device:', err)
      });
    }
  }

  assignDevice(id: number): void {
    this.deviceService.assignDevice(id).subscribe({
      next: () => this.loadDevices(),
      error: (err) => alert(err.error?.message || 'Failed to assign device.')
    });
  }

  unassignDevice(id: number): void {
    this.deviceService.unassignDevice(id).subscribe({
      next: () => this.loadDevices(),
      error: (err) => alert(err.error?.message || 'Failed to unassign device.')
    });
  }
}
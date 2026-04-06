import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Device } from '../models/device.model';

@Injectable({
  providedIn: 'root'
})
export class DeviceService {
  private apiUrl = 'http://localhost:5251/api/devices';

  constructor(private http: HttpClient) {}

  getDevices(): Observable<Device[]> {
    return this.http.get<Device[]>(this.apiUrl);
  }

  getDevice(id: number): Observable<Device> {
    return this.http.get<Device>(`${this.apiUrl}/${id}`);
  }

  createDevice(device: Device): Observable<Device> {
    return this.http.post<Device>(this.apiUrl, device);
  }

  updateDevice(id: number, device: Device): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, device);
  }

  deleteDevice(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  assignDevice(id: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/${id}/assign`, {});
  }

  unassignDevice(id: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/${id}/unassign`, {});
  }

  generateDescription(device: any): Observable<any> {
    return this.http.post('http://localhost:5251/api/ai/generate-description', device);
  }
}
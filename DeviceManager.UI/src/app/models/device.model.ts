export interface Device {
    id: number;
    name: string;
    manufacturer: string;
    type: string;
    operatingSystem: string;
    osVersion: string;
    processor: string;
    ram: number;
    description: string | null;
    userId: number | null;
    user: User | null;
}

export interface User {
    id: number;
    name: string;
    email: string;
    role: string;
    location: string;
}